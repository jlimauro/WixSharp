//css_ref ..\..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref ..\..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using WixSharp;

public class InstallScript
{
    static public void Main(string[] args)
    {
        var project =
            new Project("MyProduct",

                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new WixSharp.File(@"readme.txt")),

                new Binary(@"Fake CRT.msi"),
                new ManagedAction(@"InstallCRTAction", 
                                    Return.check, 
                                    When.Before, 
                                    Step.LaunchConditions, 
                                    Condition.NOT_Installed,
                                    Sequence.InstallUISequence));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");

        Compiler.BuildMsi(project);
    }


    [CustomAction]
    public static ActionResult InstallCRTAction(Session session)
    {
        //This can be successfully executed only from UISequence
        if (!IsCRTInstalled())
        {
            //extract CRT msi into temp directory
            var CRTMsiFile = Path.ChangeExtension(Path.GetTempFileName(), ".msi");
            SaveBinaryToFile(session, "Fake CRT.msi".Expand(), CRTMsiFile); //Expand() is needed to normalize file name into file ID

            //install CTR
            Process.Start(CRTMsiFile).WaitForExit();

            if (!IsCRTInstalled()) //there is no warranty that CRT installation succeeded
            {
                var result = MessageBox.Show("CRT is not installed.\n\nDo you want to continue without CRT?", "Prerequisites is not found", MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                    return ActionResult.UserExit;
            }
        }

        return ActionResult.Success;
    }

    static bool IsCRTInstalled()
    {
        using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{6F330B47-2577-43AD-1195-1861BA25889B}"))
            return key != null;
    }

    static void SaveBinaryToFile(Session session, string binary, string file)
    {
        //If binary is accessed this way it will raise "stream handle is not valid" exception
        //object result = session.Database.ExecuteScalar("select Data from Binary where Name = 'Fake_CRT.msi'");
        //Stream s = (Stream)result;
        //using (FileStream fs = new FileStream(@"E:\cs-script\Dev\Wix\WixSharp\Distro\Wix# Samples\Simplified Bootstrapper\Fake CRT1.msi", FileMode.Create))
        //{
        //    int Length = 256;
        //    var buffer = new Byte[Length];
        //    int bytesRead = s.Read(buffer, 0, Length);
        //    while (bytesRead > 0)
        //    {
        //        fs.Write(buffer, 0, bytesRead);
        //        bytesRead = s.Read(buffer, 0, Length);
        //    }
        //}

        //however View approach is OK
        using (var sql = session.Database.OpenView("select Data from Binary where Name = '" + binary + "'"))
        {
            sql.Execute();

            Stream stream = sql.Fetch().GetStream(1);
            
            using (var fs = new FileStream(file, FileMode.Create))
            {
                int Length = 256;
                var buffer = new Byte[Length];
                int bytesRead = stream.Read(buffer, 0, Length);
                while (bytesRead > 0)
                {
                    fs.Write(buffer, 0, bytesRead);
                    bytesRead = stream.Read(buffer, 0, Length);
                }
            }
        }
    }
}

