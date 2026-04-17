# Quick Reference Guide - Enhanced Test Reports

## 📂 Reports Folder Structure
```
Reports/
├── report_21_16_04_2026.html    ← View HTML report in browser
├── README.md                    ← Reports folder guide
└── [other report_*.html files]
```

---

## 🚀 Quick Commands

### View a Report
```bash
# Open report in browser
start Reports/report_21_16_04_2026.html

# Or use your favorite browser to open:
Reports/report_*.html
```

### Run Tests (Auto-generates Reports)
```bash
cd PlaywrightTests
dotnet test
# Reports auto-saved to: Reports/report_{timestamp}.html
```

### Manage Reports
```powershell
# List all reports
./manage-reports.ps1 -Action List

# Show statistics
./manage-reports.ps1 -Action Stats

# Organize by month
./manage-reports.ps1 -Action OrganizeByMonth

# Clean up reports older than 30 days
./manage-reports.ps1 -Action Cleanup -Days 30

# Archive reports to ZIP files
./manage-reports.ps1 -Action Archive
```

---

## 📋 Report Naming Convention

**Format:** `report_{HH}_{dd}_{MM}_{yyyy}.html`

| Component | Example | Meaning |
|-----------|---------|---------|
| HH | 21 | Hour (9 PM) |
| dd | 16 | Day (16th) |
| MM | 04 | Month (April) |
| yyyy | 2026 | Year (2026) |

**Examples:**
- `report_21_16_04_2026.html` → April 16, 2026 at 9:00 PM
- `report_09_17_04_2026.html` → April 17, 2026 at 9:00 AM

---

## 🔍 Reading a Report

### Report Sections
1. **Execution Summary** - Total, passed, failed, pass rate, duration
2. **Test Details** - For each test:
   - Status (✓ PASSED / ✗ FAILED)
   - Duration
   - Each step with logs
   - Errors (if failed)
   - Full logs
   - Stack trace (if failed)

### For PASSED Tests
✓ All steps completed successfully  
✓ All logs visible for verification  
✓ Data extracted/verified is shown  

### For FAILED Tests
✗ Shows which step failed (highlighted)  
✗ Shows error message and context  
✗ Complete logs leading to failure  
✗ Full stack trace for debugging  

---

## 💻 Integrating with Tests

### Basic Pattern
```csharp
[Test]
public async Task TestName()
{
    TestExecutionLogger.StartTest("TestName", "Category");
    try {
        // Record each step
        TestExecutionLogger.RecordStep(1, "Do something");
        // ... perform action ...
        TestExecutionLogger.LogStepInfo("Step completed");
        TestExecutionLogger.CompleteStep();
        
        // More steps...
        TestExecutionLogger.RecordStep(2, "Verify result");
        // ... verify ...
        TestExecutionLogger.CompleteStep();
        
        TestExecutionLogger.CompleteTest(true);
    }
    catch (Exception ex) {
        TestExecutionLogger.CompleteTest(false, ex.Message, ex.StackTrace);
        throw;
    }
}
```

---

## 🎨 Report Colors

| Status | Color | Meaning |
|--------|-------|---------|
| ✓ PASSED | Green (#10b981) | Test completed successfully |
| ✗ FAILED | Red (#ef4444) | Test failed |
| Header | Purple (#667eea) | Section headers |
| Logs | Dark | Log output background |

---

## 📁 Key Files

| File | Purpose |
|------|---------|
| `TestExecutionLogger.cs` | Step tracking utility |
| `EnhancedHTMLReportGenerator.cs` | Report generation |
| `manage-reports.ps1` | Report management script |
| `Reports/` | Report storage folder |
| `ReportConfig.json` | Report configuration |

---

## 🔧 Troubleshooting

### Report Not Generated
1. ✓ Run tests: `dotnet test`
2. ✓ Check Reports folder exists
3. ✓ Verify TestExecutionLogger is integrated
4. ✓ Check for errors in console output

### Can't Open Report
1. ✓ Use absolute path: `start Reports/report_21_16_04_2026.html`
2. ✓ Check file has .html extension
3. ✓ Verify browser installed

### Logs Not Showing
1. ✓ Call `TestExecutionLogger.LogStepInfo()` in test
2. ✓ Call `TestExecutionLogger.CompleteStep()` after each step
3. ✓ Verify no exceptions prevent step completion

---

## 📚 Documentation

Full documentation available in:
- `ENHANCED_REPORT_GUIDE.md` - Detailed feature guide
- `REPORTS_SETUP_SUMMARY.md` - Implementation summary
- `Reports/README.md` - Reports folder documentation
- `SETUP_COMPLETE.txt` - Implementation checklist

---

## ✅ What's Included

✓ Dedicated `/Reports` folder at project root  
✓ Enhanced HTML reports with step-by-step details  
✓ TestExecutionLogger for tracking execution  
✓ EnhancedHTMLReportGenerator for professional reports  
✓ Report management script  
✓ Configuration file  
✓ Complete documentation  
✓ Sample reports  

---

## 🎯 Usage Summary

1. **Run Tests** → `dotnet test`
2. **View Report** → Open `Reports/report_*.html`
3. **Check Logs** → Look for your step in report
4. **Debug Failures** → See failed step and error
5. **Manage Reports** → Use `manage-reports.ps1`

---

**Last Updated:** April 16, 2026  
**Status:** ✅ Ready to Use
