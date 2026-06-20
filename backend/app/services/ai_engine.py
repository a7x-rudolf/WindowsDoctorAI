import os
from groq import Groq
import json

client = Groq(api_key=os.environ.get("GROQ_API_KEY"))

def analyze_with_ai(scan_data: dict) -> dict:
    """
    Kirim data diagnostik ke Groq AI untuk analisis mendalam.
    """
    try:
        # Siapkan ringkasan data untuk dikirim ke AI
        prompt = f"""
Kamu adalah Windows Doctor AI, asisten diagnostik sistem Windows yang ahli.
Analisis data sistem berikut dan berikan:
1. Diagnosis utama (apa masalah terbesar)
2. Root cause (kenapa bisa terjadi)
3. Rekomendasi solusi (langkah konkret)
4. Tingkat urgensi (Critical/High/Medium/Low)

Data sistem:
- Windows Health Score: {scan_data.get('windows', {}).get('health_score', 'N/A')}
- Overall Health: {scan_data.get('windows', {}).get('overall_health', 'N/A')}
- Critical Issues: {json.dumps(scan_data.get('windows', {}).get('critical_issues', []), ensure_ascii=False)}
- CPU Info: {json.dumps(scan_data.get('hardware', {}).get('cpu', {}), ensure_ascii=False)}
- Memory Info: {json.dumps(scan_data.get('hardware', {}).get('memory', {}), ensure_ascii=False)}
- Disk Health: {json.dumps(scan_data.get('hardware', {}).get('disk', {}), ensure_ascii=False)}
- Hardware Findings: {json.dumps(scan_data.get('hardware', {}).get('findings', []), ensure_ascii=False)}

Berikan respons dalam format JSON seperti ini:
{{
    "diagnosis": "string - diagnosis utama",
    "root_cause": "string - penyebab utama",
    "recommendations": ["string", "string", "string"],
    "urgency": "Critical|High|Medium|Low",
    "summary": "string - ringkasan singkat untuk user"
}}

Jawab HANYA dengan JSON, tanpa teks lain.
"""

        response = client.chat.completions.create(
            model="llama-3.3-70b-versatile",
            messages=[
                {
                    "role": "system",
                    "content": "Kamu adalah ahli diagnostik sistem Windows. Selalu jawab dalam format JSON yang valid."
                },
                {
                    "role": "user",
                    "content": prompt
                }
            ],
            temperature=0.3,
            max_tokens=1000,
        )

        ai_response = response.choices[0].message.content.strip()

        # Parse JSON response
        result = json.loads(ai_response)
        result["success"] = True
        return result

    except json.JSONDecodeError:
        return {
            "success": False,
            "error": "AI response tidak valid",
            "diagnosis": "Tidak dapat menganalisis",
            "recommendations": [],
            "urgency": "Low",
            "summary": "AI analysis gagal, gunakan diagnosis manual"
        }
    except Exception as e:
        return {
            "success": False,
            "error": str(e),
            "diagnosis": "AI tidak tersedia",
            "recommendations": [],
            "urgency": "Low",
            "summary": "AI analysis tidak tersedia saat ini"
        }