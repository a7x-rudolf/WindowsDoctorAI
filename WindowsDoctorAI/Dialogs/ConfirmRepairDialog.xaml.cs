using Microsoft.UI.Xaml.Controls;
using WindowsDoctorAI.Models;
using WindowsDoctorAI.Views;

namespace WindowsDoctorAI.Dialogs;

public sealed partial class ConfirmRepairDialog : ContentDialog
{
    public ConfirmRepairDialog(RepairAction action)
    {
        this.InitializeComponent();
        ThemeService.ApplyToDialog(this);

        ActionNameText.Text = action.Name;
        ActionDescText.Text = action.Description;
        ActionTypeText.Text = action.ActionType.ToString();
        RiskLevelText.Text = action.RiskLevel;
        EstTimeText.Text = $"~{action.EstimatedTimeSeconds} seconds";
        AdminText.Text = action.RequiresAdmin ? "Yes" : "No";
        RebootText.Text = action.RequiresReboot ? "Yes" : "No";

        // Update InfoBar based on risk
        if (action.RequiresReboot)
        {
            InfoCallout.Severity = InfoBarSeverity.Warning;
            InfoCallout.Title = "System Reboot Required";
            InfoCallout.Message = "This action will require restarting your computer to take effect.";
        }
        else if (action.RiskLevel == "High")
        {
            InfoCallout.Severity = InfoBarSeverity.Error;
            InfoCallout.Title = "High Risk Action";
            InfoCallout.Message = "This is a high-risk action that may affect system stability. Proceed with caution.";
        }
        else if (action.RiskLevel == "Medium")
        {
            InfoCallout.Severity = InfoBarSeverity.Warning;
            InfoCallout.Title = "Medium Risk Action";
            InfoCallout.Message = "This action has medium risk. Review the details carefully.";
        }
        else
        {
            InfoCallout.Severity = InfoBarSeverity.Success;
            InfoCallout.Title = "Safe Action";
            InfoCallout.Message = "This action is safe and reversible.";
        }
    }
}