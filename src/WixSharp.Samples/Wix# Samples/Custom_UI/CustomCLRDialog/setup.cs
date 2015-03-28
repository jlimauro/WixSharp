using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;

public static class Script
{
    static public void Main()
    {
        //ProductActivationDialogSetup.Build();
        EmptyDialogSetup.Build();
    }
}
