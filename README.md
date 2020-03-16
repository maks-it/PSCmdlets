# PSCmdlets

I just want to share with you my custom PowerShell cmdlets collection, which every System Administrator will appreciate.

## Latest builds

* [PSCmdlets_v1.1.7380.40346](PSCmdlets_v1.1.7380.40346.zip) (Fixed issues in **Convert-ToJson** and **Convert-FromJson** cmdlets)
* [PSCmdlets_v1.1.7380.3631](builds/PSCmdlets_v1.1.7380.3631.zip)


## How to include custom cmdlets in your script

```PowerShell
    $fullPath = $MyInvocation.MyCommand.Definition
    $scriptPath = Split-Path  -Path $fullPath -Parent

    Import-Module -Name "$scriptPath\Modules\PSCmdlets\PSCmdlets.dll"
```

## Available cmdlets

Here is a full list of availavble cmdlets in this solution. I kept MS documentation style to be more clear and comprhensive.

## Get-PSCmdletsVersion

Get information about PSCmdlets version and its support Lib.

```PowerShell
    PS C:\WINDOWS\system32> Get-PSCmdletsVersion
    NOTE: Lib.dll Version: 1.0.7379.41249, PSCmdlets.dll Version: 1.0.7379.41249
```

## Get-InstalledPrograms

Collects full info about **system wide** and **user level** installed programs and all theri registry parameters.

```PowerShell
    Get-InstalledPrograms
        [-DisplayName <String>]
        [-DisplayVersion <String>]
```

### Description

The **Get-InstalledPrograms** cmdlet scans:

* @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Uninstall"
* @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
* @"HKEY_USERS\" + regUser.SID + @"\Software\Microsoft\Windows\CurrentVersion\Uninstall"

Then returns every parameter found into every program registry path.

All listed cmdlet parameters can be passed from pipeline.

### Exapmles

```PowerShell
    PS C:\WINDOWS\system32> Get-InstalledPrograms | Out-GridView

    PS C:\WINDOWS\system32> Get-InstalledPrograms -DisplayName *Acrobat* -DisplayVersion 20* | Out-GridView
```

## Uninstall-Programs

Allows to unistall program by DisplayName, DisplayVersion with Arguments

```PowerShell
    Uninstall-Programs
        [-DisplayName <String>]
        [-DisplayVersion <String>]
        [-Arguments <String>]
```

### Description

The **Uninstall-Programs** cmdlet was design to be paired with **Get-InstalledPrograms** cmdlet in pipeline or to be standalone

### Examples

```PowerShell
    PS C:\WINDOWS\system32> Get-InstalledPrograms -DisplayName *Acrobat* |  Uninstall-Programs -Arguments /qn

    PS C:\WINDOWS\system32> Uninstall-Programs -DisplayName *Acrobat* -Arguments /qn
```

## Convert-ToJson

Is an **Newtonsoft JSON** based cmdlet, with all its flexibility. I have created it, as at certain moment standard cmdlet wasn't able to convert **deep nested JSON object**

```PowerShell
    Convert-ToJson
        [-JsonObject <PSObject>]
```

### Description

The **Convert-ToJson** cmdlet is able to take nested **PSCustomObject** and return indeted json string.

### Examples

```PowerShell
    PS C:\WINDOWS\system32> $pso = [pscustomobject]@{ hi = "hey" }
    PS C:\WINDOWS\system32> $pso | Convert-ToJson
    {
        "hi": "hey"
    }
```

## Convert-FromJson

Is **Newtonsoft JSON** based cmdlet, with all its flexibility. I have created it, as at certain moment standard cmdlet wasn't able to convert **deep nested JSON object**

```PowerShell
    Convert-FromJson
        [-JsonString <String>]
```

### Description

The **Convert-FromJson** cmdlet takes input **JSON string**, which could be a `JSON Object`, or a `JSON Array`. In case of `JSON Object` it returns Newtonsoft JSON.Net **JObject** type and in case of `JSON Array` **Object[]** of **JObject**.

```PowerShell
    PS C:\WINDOWS\system32> (Convert-FromJson '[{ "hi": "hey" },  { "hi": "ciao" }, { "hi": "privet" }]').GetType()

    IsPublic IsSerial Name                                     BaseType
    -------- -------- ----                                     --------
    True     True     Object[]                                 System.Array


    PS C:\WINDOWS\system32> (Convert-FromJson '{ "hi": "hey" }').GetType()
    IsPublic IsSerial Name                                     BaseType
    -------- -------- ----                                     --------
    True     False    JObject                                  Newtonsoft.Json.Linq.JContainer
```

### Examples

```PowerShell
    # JSON Array
    PS C:\WINDOWS\system32> Convert-FromJson '[{ "hi": "hey" },  { "hi": "ciao" }, { "hi": "privet" }]' | ForEach-Object { Write-Host $_["hi"].Value }
    hey
    ciao
    privet

    # JSON Object
    PS C:\WINDOWS\system32> Write-Host (Convert-FromJson '{ "hi": "hey" }')["hi"].Value
    hey
```

## New-RegValue

```PowerShell
    New-RegValue
        [-RegPath <String>]
        [-ValueName <String>]
        [-ValueType <String>]
        [-ValueData <Object>]
```

### Description

### Examples

## Remove-UninstallKeysByVal

```PowerShell
    Remove-UninstallKeysByVal
        [-Value <String>]
```

### Description

### Examples

## Set-ActiveSetup

```PowerShell
    Set-ActiveSetup
        [-ComponentID <String>]
        [-StubPath <String>]
        [-Is32bitApp]
        [-Version <String>]
        [-Locale]
```

## Undo-ActiveSetup

```PowerShell
    Undo-ActiveSetup
        [-ComponentID <String>]
```

### Description

### Examples

## Copy-ToFolder

Copy all folder contents to another folder.

```PowerShell
    Copy-ToFolder
        [-SourcePath <String>]
        [-DestDirPath <String>]
        [-Force]
```

### Description

The **Copy-ToFolder** cmdlet had objective to provide simplier folder copy mechanism then than standatd PowerShell.

### Examples

```PowerShell
    PS C:\WINDOWS\system32>  Copy-ToFolder -SourcePath C:\Folder_1 -DestDirPath C:\Folder_2 -Force
```

## Get-DrivesInfo

List all drives information on Host.

```PowerShell
    Get-DrivesInfo
```

### Description

The **Get-DrivesInfo** cmdlet allows you to get drive information such, as DriveName, UserFreeSpace, FreeSpace, TotalSize, DriveType and DriveFormat. Size values are in bytes.

### Examples

```PowerShell
    PS C:\WINDOWS\system32> Get-DrivesInfo

    DriveName     : C:\
    UserFreeSpace : 199656325120
    FreeSpace     : 199656325120
    TotalSize     : 510917005312
    DriveType     : Fixed
    DriveFormat   : NTFS
```

## Invoke-MSSQLQuery

```PowerShell
    Invoke-MSSQLQuery
        [-ConnectionString <String>]
        [-Statement <String>]
```

## Invoke-MSSQLNonQuery

```PowerShell
    Invoke-MSSQLNonQuery
        [-ConnectionString <String>]
        [-Statement <String>]
```

### Description

### Examples

## Stop-ProcessByName

```PowerShell
    Stop-ProcessByName
        [-Name <String>]
```

### Description

### Examples

## Ping-HostTCPPort

```PowerShell
    Ping-HostTCPPort
        [-HostUri <String>]
        [-PortNumber <Int32>]
```

### Description

### Examples

## Ping-HostUDPPort

```PowerShell
    Ping-HostUDPPort
        [-HostUri <String>]
        [-PortNumber <Int32>]
```

### Description

### Examples

## Set-Culture

```PowerShell
    Set-Culture
        [-Culture <String>]
```

### Description

### Examples

## Confirm-Schedule

### Description

### Examples

```PowerShell
    PS C:\WINDOWS\system32>  Confirm-Schedule -Name "Test_1" -RunMode Daily -RunDay Friday -RunTime "13:00:00" -CurrentDateTimeUtc "2019-02-06T17:43:19.0125725Z" -ExecHistoryPath  "C:\Test\exechistory.json"
```

## Update-execHistory

### Description

### Examples

```PowerShell
    PS C:\WINDOWS\system32>  Update-ExecHistory -Name "Test_1" -ExecHistoryPath "C:\Test\exechistory.json"
```
