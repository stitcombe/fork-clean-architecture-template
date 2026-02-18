param(
    [Parameter(Mandatory = $true)]
    [string]$Name,
    [string]$Output = (Get-Location).Path
)

$ErrorActionPreference = "Stop"

$templateName = "BoricuaCoder.CleanTemplate"
$templateDbName = "cleantemplate"
$targetDir = Join-Path $Output $Name
$targetDbName = $Name.ToLowerInvariant().Replace(".", "_")
$sourceRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

New-Item -ItemType Directory -Force -Path $targetDir | Out-Null

Get-ChildItem -LiteralPath $sourceRoot -Force | Where-Object {
    $_.Name -ne ".git" -and
    $_.Name -ne ".vs" -and
    $_.FullName -ne $targetDir
} | ForEach-Object {
    Copy-Item -LiteralPath $_.FullName -Destination $targetDir -Recurse -Force
}

Get-ChildItem -LiteralPath $targetDir -Directory -Recurse -Force | Where-Object {
    $_.Name -eq "bin" -or $_.Name -eq "obj"
} | Remove-Item -Recurse -Force

Get-ChildItem -LiteralPath $targetDir -Recurse -Force |
    Sort-Object { $_.FullName.Length } -Descending |
    ForEach-Object {
        if ($_.Name -like "*$templateName*") {
            $newName = $_.Name.Replace($templateName, $Name)
            Rename-Item -LiteralPath $_.FullName -NewName $newName -Force
        }
    }

Get-ChildItem -LiteralPath $targetDir -File -Recurse -Force | ForEach-Object {
    try {
        $content = [System.IO.File]::ReadAllText($_.FullName)
    }
    catch {
        return
    }

    $updated = $content.Replace($templateName, $Name).Replace($templateDbName, $targetDbName)
    if ($updated -ne $content) {
        [System.IO.File]::WriteAllText($_.FullName, $updated)
    }
}

Write-Output "Template scaffolded at: $targetDir"
