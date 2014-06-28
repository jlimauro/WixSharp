This sample demonstrates how to implement native Win32 Bootstrapper.
Execute corresponding Build.cmd file to build the bootstrapper (setup.exe).

Bootstrapper will install prerequisite (if it is not already installed) and then automatically execurtes embedded MyProductmsi. The bootstrapper file (setup.exe) is self sufficient and does not require msi files to be distributed along.

Note: DotNETBootstrapper.cs cannot be cexecuted and it is presented for demo purposes only as an example of .NET bootstrapper.