const { app, BrowserWindow, shell, dialog, ipcMain } = require('electron');
const path = require('path');
const { spawn } = require('child_process');
const fs = require('fs');
const http = require('http');

let mainWindow;
let splashWindow;
let backendProcess;

// ── Path helpers ──────────────────────────────────────────────────────────────

function getBackendPath() {
  if (app.isPackaged) {
    return path.join(process.resourcesPath, 'backend', 'backend.exe');
  }
  return path.join(__dirname, '..', 'backend', 'dist', 'backend.exe');
}

function getFrontendPath() {
  if (app.isPackaged) {
    return path.join(process.resourcesPath, 'frontend', 'index.html');
  }
  return path.join(__dirname, '..', 'frontend', 'dist', 'index.html');
}

// ── Splash screen ─────────────────────────────────────────────────────────────

function createSplash() {
  splashWindow = new BrowserWindow({
    width: 420,
    height: 280,
    frame: false,
    transparent: true,
    alwaysOnTop: true,
    resizable: false,
    center: true,
    skipTaskbar: true,
    webPreferences: { nodeIntegration: false, contextIsolation: true },
  });

  // Inline HTML splash — tidak butuh file terpisah
  const splashHTML = `
    <!DOCTYPE html>
    <html>
    <head>
      <meta charset="UTF-8"/>
      <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body {
          background: #0a0e13;
          border: 1px solid #1f2630;
          border-radius: 16px;
          display: flex;
          flex-direction: column;
          align-items: center;
          justify-content: center;
          height: 100vh;
          font-family: 'Segoe UI', system-ui, sans-serif;
          color: #e6edf3;
          overflow: hidden;
        }
        .logo {
          width: 56px; height: 56px;
          background: rgba(45,212,191,0.12);
          border-radius: 14px;
          display: flex; align-items: center; justify-content: center;
          margin-bottom: 16px;
          font-size: 26px;
        }
        h1 { font-size: 20px; font-weight: 700; margin-bottom: 4px; }
        .sub { font-size: 12px; color: #4d5868; margin-bottom: 32px; letter-spacing: 0.08em; text-transform: uppercase; }
        .status { font-size: 12px; color: #8b97a6; margin-bottom: 16px; }
        .bar-track {
          width: 260px; height: 3px;
          background: #1f2630; border-radius: 99px; overflow: hidden;
        }
        .bar-fill {
          height: 100%; width: 0%;
          background: linear-gradient(90deg, #2dd4bf, #14b8a6);
          border-radius: 99px;
          animation: load 3s ease-in-out forwards;
        }
        @keyframes load {
          0%   { width: 0%; }
          40%  { width: 60%; }
          80%  { width: 85%; }
          100% { width: 95%; }
        }
        .version { position: absolute; bottom: 16px; font-size: 10px; color: #2a3340; }
      </style>
    </head>
    <body>
      <div class="logo">🩺</div>
      <h1>Windows Doctor AI</h1>
      <div class="sub">AI Diagnostics</div>
      <div class="status" id="status">Starting backend…</div>
      <div class="bar-track"><div class="bar-fill"></div></div>
      <div class="version">v2.0.0 · Developed by Rudolf</div>
      <script>
        const statuses = [
          'Starting backend…',
          'Initializing diagnostic engine…',
          'Loading system collectors…',
          'Connecting to Windows API…',
          'Almost ready…',
        ];
        let i = 0;
        const el = document.getElementById('status');
        const interval = setInterval(() => {
          i++;
          if (i < statuses.length) el.textContent = statuses[i];
          else clearInterval(interval);
        }, 700);
      </script>
    </body>
    </html>
  `;

  splashWindow.loadURL(`data:text/html;charset=utf-8,${encodeURIComponent(splashHTML)}`);
}

function closeSplash() {
  if (splashWindow && !splashWindow.isDestroyed()) {
    splashWindow.close();
    splashWindow = null;
  }
}

// ── Backend ───────────────────────────────────────────────────────────────────

//fungsi startBackend()
function startBackend(retryCount = 0) {
  const backendPath = getBackendPath();

  if (!fs.existsSync(backendPath)) {
    dialog.showErrorBox('Backend tidak ditemukan', `File backend.exe tidak ditemukan di:\n${backendPath}`);
    return;
  }

  backendProcess = spawn(backendPath, [], {
    detached: false,
    stdio: 'ignore',
    windowsHide: true,
  });

  backendProcess.on('error', (err) => {
    console.error('Gagal menjalankan backend:', err);
  });

  backendProcess.on('exit', (code) => {
    console.log('Backend process keluar dengan kode:', code);
    // Auto-restart jika crash (bukan karena app ditutup), max 3x
    if (code !== 0 && mainWindow && !mainWindow.isDestroyed() && retryCount < 3) {
      console.log(`Restart backend... (percobaan ${retryCount + 1})`);
      setTimeout(() => startBackend(retryCount + 1), 2000);
    }
  });
}

function waitForBackend(callback, maxRetries = 40) {
  let retries = 0;

  const check = () => {
    const req = http.get('http://127.0.0.1:8000/', (res) => {
      console.log('Backend siap! Status:', res.statusCode);
      callback(true);
    });

    req.on('error', () => {
      if (retries < maxRetries) {
        retries++;
        setTimeout(check, 500);
      } else {
        console.warn('Backend tidak merespons setelah', maxRetries, 'percobaan. Tetap buka window.');
        callback(false);
      }
    });

    req.setTimeout(400, () => {
      req.destroy();
    });
  };

  // Beri backend sedikit waktu untuk mulai sebelum polling
  setTimeout(check, 800);
}

// ── Main window ───────────────────────────────────────────────────────────────

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 1280,
    height: 800,
    minWidth: 1024,
    minHeight: 640,
    title: 'Windows Doctor AI',
    icon: path.join(__dirname, 'icon.ico'),
    show: false, // tampilkan setelah ready
    backgroundColor: '#0a0e13',
    webPreferences: {
      nodeIntegration: false,
      contextIsolation: true,
      webSecurity: true,
    },
    autoHideMenuBar: true,
  });

  const frontendPath = getFrontendPath();

  if (!fs.existsSync(frontendPath)) {
    dialog.showErrorBox(
      'Frontend tidak ditemukan',
      `File index.html tidak ditemukan di:\n${frontendPath}\n\nJalankan "npm run build" di folder frontend terlebih dahulu.`
    );
    app.quit();
    return;
  }

  mainWindow.loadFile(frontendPath);

  // Tampilkan window setelah konten siap, tutup splash
  mainWindow.once('ready-to-show', () => {
    closeSplash();
    mainWindow.show();
    mainWindow.focus();
  });

  // Buka link eksternal di browser default
  mainWindow.webContents.setWindowOpenHandler(({ url }) => {
    shell.openExternal(url);
    return { action: 'deny' };
  });

  mainWindow.on('closed', () => {
    mainWindow = null;
  });
}

// ── Kill backend helper ───────────────────────────────────────────────────────

function killBackend() {
  if (backendProcess && !backendProcess.killed) {
    try {
      // Paksa kill seluruh process tree di Windows
      spawn('taskkill', ['/pid', backendProcess.pid.toString(), '/f', '/t'], {
        windowsHide: true,
        stdio: 'ignore',
      });
    } catch (e) {
      backendProcess.kill();
    }
    backendProcess = null;
  }
}

// ── App lifecycle ─────────────────────────────────────────────────────────────

app.whenReady().then(() => {
  createSplash();
  startBackend();
  waitForBackend(() => {
    createWindow();
  });
});

app.on('window-all-closed', () => {
  killBackend();
  app.quit();
});

app.on('before-quit', () => {
  killBackend();
});

// Pastikan backend mati kalau ada crash / force close
process.on('exit', killBackend);
process.on('SIGINT', () => { killBackend(); process.exit(0); });
process.on('uncaughtException', (err) => {
  console.error('Uncaught exception:', err);
  killBackend();
});