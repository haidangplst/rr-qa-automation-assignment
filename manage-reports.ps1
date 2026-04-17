#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Manages HTML test reports - organize, archive, and clean up old reports

.DESCRIPTION
    This script provides utilities to:
    - List all reports
    - Archive old reports by month
    - Clean up reports older than retention period
    - Generate summary statistics

.PARAMETER Action
    Action to perform: List, Archive, Cleanup, Stats, OrganizeByMonth

.EXAMPLE
    .\manage-reports.ps1 -Action List
    .\manage-reports.ps1 -Action Stats
    .\manage-reports.ps1 -Action OrganizeByMonth
    .\manage-reports.ps1 -Action Cleanup -Days 30
#>

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet('List', 'Stats', 'Archive', 'Cleanup', 'OrganizeByMonth')]
    [string]$Action,

    [Parameter(Mandatory=$false)]
    [int]$Days = 30,

    [Parameter(Mandatory=$false)]
    [string]$ReportsPath = "Reports"
)

# Ensure Reports directory exists
if (-not (Test-Path $ReportsPath)) {
    Write-Host "Creating Reports directory: $ReportsPath" -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $ReportsPath -Force | Out-Null
}

function List-Reports {
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "📋 Available Test Reports" -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan

    $reports = Get-ChildItem -Path $ReportsPath -Filter "report_*.html" -File | Sort-Object CreationTime -Descending

    if ($reports.Count -eq 0) {
        Write-Host "No reports found in $ReportsPath" -ForegroundColor Yellow
        return
    }

    $reports | ForEach-Object {
        $sizeKB = [Math]::Round($_.Length / 1024, 2)
        $age = [Math]::Round((New-TimeSpan -Start $_.CreationTime -End (Get-Date)).TotalDays, 1)

        $ageColor = if ($age -lt 1) { 'Green' } elseif ($age -lt 7) { 'Yellow' } else { 'Gray' }

        Write-Host "  ✓ $($_.Name)" -ForegroundColor Green
        Write-Host "    Created: $($_.CreationTime.ToString('yyyy-MM-dd HH:mm:ss'))" -ForegroundColor Gray
        Write-Host "    Size: $($sizeKB) KB | Age: $($age) days" -ForegroundColor Gray
    }

    Write-Host "`nTotal reports: $($reports.Count)" -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan
}

function Show-Stats {
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "📊 Report Statistics" -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan

    $reports = Get-ChildItem -Path $ReportsPath -Filter "report_*.html" -File

    if ($reports.Count -eq 0) {
        Write-Host "No reports found" -ForegroundColor Yellow
        return
    }

    $totalSize = ($reports | Measure-Object -Property Length -Sum).Sum
    $totalSizeMB = [Math]::Round($totalSize / 1024 / 1024, 2)

    $oldest = $reports | Sort-Object CreationTime | Select-Object -First 1
    $newest = $reports | Sort-Object CreationTime -Descending | Select-Object -First 1

    $avgSize = [Math]::Round(($reports | Measure-Object -Property Length -Average).Average / 1024, 2)

    Write-Host "  Total Reports: $($reports.Count)" -ForegroundColor Green
    Write-Host "  Total Size: $($totalSizeMB) MB" -ForegroundColor Green
    Write-Host "  Average Size: $($avgSize) KB" -ForegroundColor Green
    Write-Host ""
    Write-Host "  Oldest Report: $($oldest.Name)" -ForegroundColor Yellow
    Write-Host "    Created: $($oldest.CreationTime.ToString('yyyy-MM-dd HH:mm:ss'))" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  Newest Report: $($newest.Name)" -ForegroundColor Yellow
    Write-Host "    Created: $($newest.CreationTime.ToString('yyyy-MM-dd HH:mm:ss'))" -ForegroundColor Gray

    Write-Host "`n========================================`n" -ForegroundColor Cyan
}

function Organize-ByMonth {
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "📁 Organizing Reports by Month" -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan

    $reports = Get-ChildItem -Path $ReportsPath -Filter "report_*.html" -File

    if ($reports.Count -eq 0) {
        Write-Host "No reports to organize" -ForegroundColor Yellow
        return
    }

    foreach ($report in $reports) {
        # Parse report filename: report_HH_dd_MM_yyyy.html
        if ($report.Name -match "report_(\d{2})_(\d{2})_(\d{2})_(\d{4})\.html") {
            $month = $matches[3]
            $year = $matches[4]
            $folderName = "$year-$month"
            $folderPath = Join-Path $ReportsPath $folderName

            if (-not (Test-Path $folderPath)) {
                New-Item -ItemType Directory -Path $folderPath -Force | Out-Null
                Write-Host "  Created folder: $folderName" -ForegroundColor Green
            }

            $newPath = Join-Path $folderPath $report.Name
            if (-not (Test-Path $newPath)) {
                Move-Item -Path $report.FullName -Destination $newPath
                Write-Host "  Moved: $($report.Name) → $folderName/" -ForegroundColor Green
            }
        }
    }

    Write-Host "`nOrganization complete!" -ForegroundColor Green
    Write-Host "========================================`n" -ForegroundColor Cyan
}

function Cleanup-OldReports {
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "🗑️  Cleaning Up Old Reports" -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan

    Write-Host "Retention period: $Days days`n" -ForegroundColor Yellow

    $reports = Get-ChildItem -Path $ReportsPath -Filter "report_*.html" -File
    $cutoffDate = (Get-Date).AddDays(-$Days)

    $oldReports = $reports | Where-Object { $_.CreationTime -lt $cutoffDate }

    if ($oldReports.Count -eq 0) {
        Write-Host "No reports older than $Days days found" -ForegroundColor Green
        Write-Host "========================================`n" -ForegroundColor Cyan
        return
    }

    Write-Host "Found $($oldReports.Count) report(s) older than $Days days:`n" -ForegroundColor Yellow

    $oldReports | ForEach-Object {
        $age = [Math]::Round((New-TimeSpan -Start $_.CreationTime -End (Get-Date)).TotalDays, 1)
        Write-Host "  × $($_.Name) (Age: $age days)" -ForegroundColor Red
    }

    Write-Host ""
    $confirm = Read-Host "Delete these reports? (y/n)"

    if ($confirm -eq 'y') {
        $oldReports | ForEach-Object {
            Remove-Item -Path $_.FullName -Force
            Write-Host "  Deleted: $($_.Name)" -ForegroundColor Green
        }
        Write-Host "`nCleanup completed!" -ForegroundColor Green
    } else {
        Write-Host "Cleanup cancelled" -ForegroundColor Yellow
    }

    Write-Host "========================================`n" -ForegroundColor Cyan
}

function Archive-Reports {
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "📦 Archiving Reports" -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan

    $monthFolders = Get-ChildItem -Path $ReportsPath -Directory | Where-Object { $_.Name -match "^\d{4}-\d{2}$" }

    if ($monthFolders.Count -eq 0) {
        Write-Host "No month folders found to archive" -ForegroundColor Yellow
        return
    }

    foreach ($folder in $monthFolders) {
        $archiveName = "Reports_$($folder.Name).zip"
        $archivePath = Join-Path $ReportsPath $archiveName

        if (-not (Test-Path $archivePath)) {
            Write-Host "Creating archive: $archiveName" -ForegroundColor Green

            # Check if 7z or Compress-Archive is available
            try {
                Compress-Archive -Path $folder.FullName -DestinationPath $archivePath -CompressionLevel Optimal
                Write-Host "  ✓ Archive created successfully" -ForegroundColor Green
                Write-Host "  Size: $([Math]::Round((Get-Item $archivePath).Length / 1024 / 1024, 2)) MB" -ForegroundColor Gray
            }
            catch {
                Write-Host "  ✗ Failed to create archive: $_" -ForegroundColor Red
            }
        }
    }

    Write-Host "`nArchive creation complete!" -ForegroundColor Green
    Write-Host "========================================`n" -ForegroundColor Cyan
}

# Execute action
switch ($Action) {
    'List' { List-Reports }
    'Stats' { Show-Stats }
    'Archive' { Archive-Reports }
    'Cleanup' { Cleanup-OldReports }
    'OrganizeByMonth' { Organize-ByMonth }
}
