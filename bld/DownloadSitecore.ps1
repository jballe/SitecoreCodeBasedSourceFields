param([parameter(Mandatory)]$username, [parameter(Mandatory)]$password) 

#Sitecore 8.1 Update 1
$downloadurl = "https://dev.sitecore.net/~/media/607AA63EF66E45D0828023CD8E127D88.ashx"
$versionfolder = "Sitecore 8.1 rev. 151207" # Sitecore 8.1 Update-1

#Sitecore 8.1 Update 2
#$downloadurl = "https://dev.sitecore.net/~/media/4AD00668158F4960B7C76B0C7A1B6EED.ashx"
#$versionfolder = "Sitecore 8.1 rev. 160302"

$zipfile = (Join-Path $PSScriptRoot "Sitecore.zip")
$Destination = (Join-Path $PSScriptRoot "..\src\Website" -Resolve)

function DownloadZip
{
    if($username -eq $null -or $password -eq $null) {
        Write-Warning "Username and password required to download zipfile. Specify as arguments"
        return
    }

    $credentials = @{
        username = $username
        password = $password 
    }

    $loginurl = "https://dev.sitecore.net/api/authorization" 
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

    write-debug "Login to $loginurl with user $username"
    $login = Invoke-RestMethod -Method Post -Uri $loginurl -Body (ConvertTo-Json $credentials) -ContentType "application/json;charset=UTF-8" -WebSession $session
    If($login -ne $True) {
        Write-Warning "Incorrect username or password"
        return
    }
    
    Write-Host "Start download of file"
    Invoke-WebRequest -Uri $downloadurl -WebSession $session -OutFile $zipfile -TimeoutSec (60 * 10)
}

# Extract
[Reflection.Assembly]::LoadWithPartialName('System.IO.Compression.FileSystem') | Out-Null
function ExtractPartOfZip($zipfile, $folder, $dst, $overwrite)
{    
    Write-Host "Extracting $folder folder to $dst"
    $filter = "${versionfolder}/${folder}/*"
    write-debug "Filter: $filter"
    [IO.Compression.ZipFile]::OpenRead($zipfile).Entries | ? {
      $_.FullName -like $filter
    } | % {
      $index = $_.FullName.indexOf($folder) + $folder.Length + 1
      $file  = Join-Path $dst $_.FullName.Substring($index)

      $exists = (Test-Path $file)
      If($overwrite -or -not $exists) {
        $parent = Split-Path -Parent $file
        if (-not (Test-Path -Path $parent)) {
            New-Item -Path $parent -Type Directory | Out-Null
        }
        
        If($file.EndsWith("\") -eq $false) {
            Try {
                Write-Debug ("Will copy " + $_.FullName + " to " + $file)
                [IO.Compression.ZipFileExtensions]::ExtractToFile($_, $file, $overwrite)
            } Catch {
                Write-Warning ("Error while copying " + $file + ": " + $_.Exception.Message)
            }
        }
      }
    }
}

function ExtractZipFile 
{
    Write-Host "Extracting content of zipfile to proper locations"
    ExtractPartOfZip -zipfile $zipfile -folder "Website" -dst $Destination -overwrite $false
    ExtractPartOfZip -zipfile $zipfile -folder "Data" -dst (Join-Path $Destination "App_Data\Sitecore") -overwrite $false
    ExtractPartOfZip -zipfile $zipfile -folder "Databases" -dst (Join-Path $Destination "App_Data\Databases") -overwrite $false
}

if((Test-Path $zipfile) -ne $True) {
    DownloadZip
} else {
    Write-Host "Sitecore zip file already exists, will use that"
}

if((Test-Path $zipfile) -ne $True) {
    Write-Warning "No downloaded Sitecore available"
} else {
    ExtractZipFile
}