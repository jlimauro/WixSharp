using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WixSharp;

namespace ConsoleApplication1
{
    public partial class ProductActivationForm : WixForm
    {
        public ProductActivationForm()
        {
            InitializeComponent();
            //NextButton.Conditions.Add(new WixControlCondition { Action = ConditionAction.enable, Value = "USE_ACTIVATION=\"1\"" });
            //NextButton.Conditions.Add(new WixControlCondition { Action = ConditionAction.disable, Value = "NOT (USE_ACTIVATION=\"1\")" });
        }

        void CancelButton_Click()
        {
            //normally starting the next dialog is done outside of the dialog (in the publish elements of the UI element)
            //this.EndDialog(EndDialogValue.Exit);
        }

        void NextButton_Click()
        {
            //normally starting the next dialog is done outside of the dialog (in the publish elements of the UI element)
            //this.EndDialog(EndDialogValue.Return);
            //or
            //this.Do(ControlAction.EndDialog, EndDialogValue.Return); 
            //or
            //this.Do("EndDialog", "Return"); 
        }

        void BackButton_Click()
        {
            //normally starting the next dialog is done outside of the dialog (in the publish elements of the UI element)
            //otherwise it can be done directly in the event handler
            //this.Do(ControlAction.NewDialog, "LicenseAgreementDlg");
        }

        private void wixButton_Click()
        {
            this.Do(ControlAction.DoAction, "ClaimLicenceKey");
        }
    }
}
