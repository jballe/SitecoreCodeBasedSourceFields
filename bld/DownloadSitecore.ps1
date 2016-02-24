param($username, $password) 

$zipfile = (Join-Path $PWD "Sitecore.zip")

# Sitecore 8.1 Update-1

function DownloadZip
{
    if($username -eq $null -or $password -eq $null) {
        write-error "Username and password required to download zipfile"
        return
    }

    $credentials = @{
        username = $username
        password = $password 
    }
    $url = "https://dev.sitecore.net/~/media/607AA63EF66E45D0828023CD8E127D88.ashx"
    $loginurl = "https://dev.sitecore.net/api/authorization"
 
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    Invoke-RestMethod -Method Post -Uri $loginurl -Body (ConvertTo-Json $credentials) -ContentType "application/json;charset=UTF-8" -WebSession $session
    Write-Host "Download file"
    Invoke-WebRequest -Uri $url -WebSession $session -OutFile $zipfile -TimeoutSec (60 * 10)
}

# Extract
[Reflection.Assembly]::LoadWithPartialName('System.IO.Compression.FileSystem') | Out-Null
function ExtractPartOfZip($zipfile, $folder, $dst, $overwrite)
{
    [IO.Compression.ZipFile]::OpenRead($zipfile).Entries | ? {
      $_.FullName -like "$($folder -replace '\\','/')/*"
    } | % {
      $file   = Join-Path $dst $_.FullName
      $parent = Split-Path -Parent $file
      if (-not (Test-Path -LiteralPath $parent)) {
        New-Item -Path $parent -Type Directory | Out-Null
      }
      [IO.Compression.ZipFileExtensions]::ExtractToFile($_, $file, $overwrite)
    }
}

function ExtractZipFile 
{
    Write-Host "Extracting content of zipfile to proper locations"
    $Destination = Join-Path ($PWD) "..\src\Website"
    ExtractPartOfZip -zipfile $zipfile -folder "Website" -dst $Destination -overwrite $false
    ExtractPartOfZip -zipfile $zipfile -folder "Data" -dst (Join-Path $Destination "App_Data\Sitecore") -overwrite $false
    ExtractPartOfZip -zipfile $zipfile -folder "Database" -dst (Join-Path $Destination "App_Data\Databases") -overwrite $false
}

if((Test-Path $zipfile) -ne $True) {
    DownloadZip
}

ExtractZipFIle