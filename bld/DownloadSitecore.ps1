param($username, $password) 

$zipfile = (Join-Path $PWD "Sitecore.zip")

# Sitecore 8.1 Update-1

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
    $url = "https://dev.sitecore.net/~/media/607AA63EF66E45D0828023CD8E127D88.ashx"
    $loginurl = "https://dev.sitecore.net/api/authorization"
 
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    $login = Invoke-RestMethod -Method Post -Uri $loginurl -Body (ConvertTo-Json $credentials) -ContentType "application/json;charset=UTF-8" -WebSession $session
    If($login -eq "False") {
        Write-Warning "Incorrect username or password"
        return
    }
    
    Write-Host "Start download of file"
    Invoke-WebRequest -Uri $url -WebSession $session -OutFile $zipfile -TimeoutSec (60 * 10)
}

# Extract
[Reflection.Assembly]::LoadWithPartialName('System.IO.Compression.FileSystem') | Out-Null
function ExtractPartOfZip($zipfile, $folder, $dst, $overwrite)
{
    Write-Host "Extracting $folder folder to $dst"
    $filter = "*/$folder/*"
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
        
        Try {
            [IO.Compression.ZipFileExtensions]::ExtractToFile($_, $file, $overwrite)
            Write-Debug ("Copied " + $_.FullName + " to " + $file)
        } Catch {
            If($file.EndsWith("\") -eq $false) {
                Write-Warning ("Error while copying " + $file + ": " + $_.Exception.Message)
            }
        }
      }
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

if((Test-Path $zipfile) -ne $True) {
    Write-Warning "No downloaded Sitecore available"
} else {
    ExtractZipFIle
}