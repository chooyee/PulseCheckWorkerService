$acl = Get-Acl "C:\Users\chooy\source\repos\PulseCheckWorkerService\PulseCheckWorkerService\bin\Release\net6.0\publish\win-x64"
$aclRuleArgs = "LocalSystem", "Read,Write,ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($aclRuleArgs)
$acl.SetAccessRule($accessRule)
$acl | Set-Acl "C:\Users\chooy\source\repos\PulseCheckWorkerService\PulseCheckWorkerService\bin\Release\net6.0\publish\win-x64"

New-Service -Name PulseCheckWorkerService -BinaryPathName "C:\Users\chooy\source\repos\PulseCheckWorkerService\PulseCheckWorkerService\bin\Release\net6.0\publish\win-x64\PulseCheckWorkerService.exe --contentRoot C:\Users\chooy\source\repos\PulseCheckWorkerService\PulseCheckWorkerService\bin\Release\net6.0\publish\win-x64" -Description "PulseCheckWorkerService" -DisplayName "PulseCheckWorkerService" -StartupType Automatic