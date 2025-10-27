# GitHub Actions Workflows

## Test and Report Workflow

**File:** `test-and-report.yml`

### What It Does

This workflow automatically:
1. ✅ Runs all tests (Unit, UI, BDD) on every push
2. 📊 Generates Allure report with historical trends
3. 🌐 Publishes report to GitHub Pages
4. 📈 Preserves test history across runs for trend analysis

### Triggers

- **Push** to `master` or `main` branch
- **Pull Request** to `master` or `main` branch
- **Manual** trigger via GitHub Actions UI

### Report URL

Once the workflow runs successfully, your Allure report will be available at:

**https://siarheistar.github.io/SDET_CS_TEST/**

### Features

#### Historical Trends
- ✓ Test execution history preserved across runs
- ✓ Duration trends over time
- ✓ Pass/fail rate charts
- ✓ Flaky test detection
- ✓ Keeps last 20 report histories

#### Test Execution
- ✓ Runs all 34 tests (11 Unit + 14 UI + 9 BDD)
- ✓ Uses Release configuration for optimal performance
- ✓ Playwright browsers installed automatically
- ✓ Continues even if tests fail to show results

#### Report Publishing
- ✓ Automatic deployment to GitHub Pages
- ✓ Report link added to GitHub Actions summary
- ✓ Available ~1 minute after workflow completes

### Workflow Steps

1. **Checkout & Setup**
   - Checkout repository
   - Setup .NET 8.0
   - Restore NuGet packages

2. **Build & Test**
   - Build solution in Release mode
   - Install Playwright browsers
   - Run all tests

3. **Generate Report**
   - Move results to project root
   - Restore previous history from gh-pages
   - Generate Allure report with trends
   - Save new history

4. **Publish**
   - Deploy report to GitHub Pages
   - Add report link to workflow summary

### First Run

On the first run, there will be no trends yet. After 2+ runs, you'll see:
- Duration trends
- Pass/fail rate over time
- Historical comparisons
- Flaky test detection

### Manual Trigger

To manually trigger the workflow:
1. Go to: https://github.com/siarheistar/SDET_CS_TEST/actions
2. Click "Run Tests and Publish Allure Report"
3. Click "Run workflow"

### Troubleshooting

**Report not showing?**
- Check workflow ran successfully in Actions tab
- Wait ~1 minute for GitHub Pages deployment
- Verify GitHub Pages is enabled in repository settings

**No trends showing?**
- Trends appear after 2+ workflow runs
- Check that history is being preserved (look for "Restoring Allure history" in logs)

**Tests failing?**
- Workflow continues even if tests fail
- Check test results in Allure report
- Review workflow logs for error details

### Local Testing

To test locally before pushing:
```bash
./run-tests.sh
```

This mirrors the CI process but runs on your machine.
