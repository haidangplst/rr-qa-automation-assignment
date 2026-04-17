# CI/CD Integration Approach for TMDB QA Automation Suite

## Overview

This document outlines the strategy for integrating the TMDB Discover test automation suite into a continuous integration/continuous deployment (CI/CD) pipeline.

## Architecture

### Pipeline Stages

```
┌─────────────────────────────────────────────────────────────────┐
│  1. Source Control                                              │
│     (Push to main/feature branch)                               │
└────────────────────┬────────────────────────────────────────────┘
                     │
┌─────────────────────────────────────────────────────────────────┐
│  2. Build & Setup                                               │
│     - Restore dependencies                                      │
│     - Install Playwright browsers                               │
│     - Build project                                             │
└────────────────────┬────────────────────────────────────────────┘
                     │
┌─────────────────────────────────────────────────────────────────┐
│  3. Test Execution (Parallel Groups)                            │
│     ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────┐        │
│     │ Smoke   │  │Category │  │Type     │  │Negative │        │
│     │ Tests   │  │ Filter  │  │ Filter  │  │ Tests   │        │
│     │ (5m)    │  │ (8m)    │  │ (8m)    │  │ (5m)    │        │
│     └─────────┘  └─────────┘  └─────────┘  └─────────┘        │
└────────────────────┬────────────────────────────────────────────┘
                     │
┌─────────────────────────────────────────────────────────────────┐
│  4. Report Generation                                           │
│     - HTML Report                                               │
│     - Console Output                                            │
│     - Log Files                                                 │
│     - Screenshots (if failed)                                   │
└────────────────────┬────────────────────────────────────────────┘
                     │
┌─────────────────────────────────────────────────────────────────┐
│  5. Results Publishing                                          │
│     - Publish reports                                           │
│     - Create defect issues (if needed)                          │
│     - Notify team                                               │
└────────────────────┬────────────────────────────────────────────┘
                     │
┌─────────────────────────────────────────────────────────────────┐
│  6. Deployment Decision                                         │
│     Success: Proceed to deployment                              │
│     Failure: Block and notify team                              │
└─────────────────────────────────────────────────────────────────┘
```

---

## Implementation Examples

### GitHub Actions

```yaml
name: TMDB QA Automation

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]
  schedule:
    - cron: '0 2 * * *'  # Daily at 2 AM

jobs:
  test:
    runs-on: ubuntu-latest
    strategy:
      matrix:
        test-group: [smoke, category-filters, type-filters, pagination, negative]

    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'

      - name: Install Playwright Browsers
        run: |
          dotnet tool install --global Microsoft.Playwright.CLI
          playwright install chromium

      - name: Restore Dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build

      - name: Run Tests - ${{ matrix.test-group }}
        run: |
          dotnet test --filter "Category=${{ matrix.test-group }}" \
            --logger "console;verbosity=detailed" \
            --logger "html;LogFileName=report-${{ matrix.test-group }}.html"

      - name: Upload Reports
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: test-reports
          path: |
            **/bin/Debug/**/TestResults/**

      - name: Upload Logs
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: test-logs
          path: Logs/**

      - name: Comment on PR
        if: always() && github.event_name == 'pull_request'
        uses: actions/github-script@v6
        with:
          script: |
            const fs = require('fs');
            const report = fs.readFileSync('Reports/latest.html', 'utf8');
            github.rest.issues.createComment({
              issue_number: context.issue.number,
              owner: context.repo.owner,
              repo: context.repo.repo,
              body: '📊 Test Report Generated\n[View Full Report](link-to-artifact)'
            });
```

### Azure Pipelines

```yaml
trigger:
  - main
  - develop

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Debug'
  dotnetVersion: '10.0.x'

stages:
  - stage: Build
    jobs:
      - job: BuildAndSetup
        steps:
          - task: UseDotNet@2
            inputs:
              version: $(dotnetVersion)

          - task: DotNetCoreCLI@2
            inputs:
              command: 'restore'
              projects: '**/*.csproj'

          - task: DotNetCoreCLI@2
            inputs:
              command: 'build'
              arguments: '--configuration $(buildConfiguration)'

          - task: PowerShell@2
            inputs:
              targetType: 'inline'
              script: |
                dotnet tool install --global Microsoft.Playwright.CLI
                playwright install chromium

  - stage: Test
    dependsOn: Build
    jobs:
      - job: SmokeTests
        displayName: 'Smoke Tests'
        steps:
          - task: DotNetCoreCLI@2
            inputs:
              command: 'test'
              arguments: '--filter "Category=Smoke" --logger trx --logger "html;LogFileName=$(Agent.TempDirectory)/smoke-test.html"'

      - job: FullTestSuite
        displayName: 'Full Test Suite'
        steps:
          - task: DotNetCoreCLI@2
            inputs:
              command: 'test'
              arguments: '--logger trx --logger "html;LogFileName=$(Agent.TempDirectory)/full-test.html"'

      - task: PublishTestResults@2
        inputs:
          testResultsFormat: 'VSTest'
          testResultsFiles: '**/TestResults/*.trx'

      - task: PublishBuildArtifacts@1
        inputs:
          pathToPublish: '$(Agent.TempDirectory)'
          artifactName: 'Test Reports'

  - stage: Decision
    dependsOn: Test
    condition: always()
    jobs:
      - job: EvaluateResults
        steps:
          - task: PowerShell@2
            inputs:
              targetType: 'inline'
              script: |
                $testsPassed = ... # Check test results
                if ($testsPassed) {
                  Write-Host "Tests passed, proceeding with deployment"
                } else {
                  Write-Host "##vso[task.logissue type=error]Tests failed"
                  exit 1
                }
```

### Jenkins Pipeline

```groovy
pipeline {
    agent any

    parameters {
        choice(name: 'TEST_ENV', choices: ['staging', 'qa'], description: 'Test Environment')
        booleanParam(name: 'GENERATE_REPORT', defaultValue: true, description: 'Generate HTML Report')
    }

    options {
        timeout(time: 30, unit: 'MINUTES')
        timestamps()
        buildDiscarder(logRotator(numToKeepStr: '10'))
    }

    environment {
        WORKSPACE_DIR = '/tmp/tmdb-tests'
        REPORT_DIR = '/var/reports'
        PLAYWRIGHT_HEADLESS = 'true'
    }

    stages {
        stage('Setup') {
            steps {
                script {
                    sh '''
                        mkdir -p ${WORKSPACE_DIR}
                        cd ${WORKSPACE_DIR}
                        dotnet restore
                        dotnet tool install --global Microsoft.Playwright.CLI
                        playwright install chromium
                    '''
                }
            }
        }

        stage('Smoke Tests') {
            steps {
                script {
                    sh '''
                        cd ${WORKSPACE_DIR}
                        dotnet test --filter "Category=Smoke" \
                            --logger "console;verbosity=normal" \
                            --logger "html;LogFileName=${REPORT_DIR}/smoke-tests.html"
                    '''
                }
            }
        }

        stage('Full Test Suite') {
            parallel {
                stage('Category Filters') {
                    steps {
                        sh '''
                            cd ${WORKSPACE_DIR}
                            dotnet test --filter "Category=CategoryFilters" \
                                --logger "html;LogFileName=${REPORT_DIR}/category-filters.html"
                        '''
                    }
                }
                stage('Type Filters') {
                    steps {
                        sh '''
                            cd ${WORKSPACE_DIR}
                            dotnet test --filter "Category=TypeFilters" \
                                --logger "html;LogFileName=${REPORT_DIR}/type-filters.html"
                        '''
                    }
                }
                stage('Pagination') {
                    steps {
                        sh '''
                            cd ${WORKSPACE_DIR}
                            dotnet test --filter "Category=Pagination" \
                                --logger "html;LogFileName=${REPORT_DIR}/pagination.html"
                        '''
                    }
                }
                stage('Negative Tests') {
                    steps {
                        sh '''
                            cd ${WORKSPACE_DIR}
                            dotnet test --filter "Category=NegativeTests" \
                                --logger "html;LogFileName=${REPORT_DIR}/negative-tests.html"
                        '''
                    }
                }
            }
        }

        stage('Report Generation') {
            when {
                expression { params.GENERATE_REPORT == true }
            }
            steps {
                script {
                    sh '''
                        cd ${WORKSPACE_DIR}
                        echo "Generating consolidated report..."
                    '''
                }
                publishHTML([
                    reportDir: '${REPORT_DIR}',
                    reportFiles: 'index.html',
                    reportName: 'TMDB QA Report'
                ])
            }
        }

        stage('Archive Results') {
            steps {
                archiveArtifacts artifacts: '${REPORT_DIR}/**', fingerprint: true
                archiveArtifacts artifacts: 'Logs/**', fingerprint: true
            }
        }
    }

    post {
        always {
            junit allowEmptyResults: true, testResults: '**/TestResults/*.xml'
            
            script {
                def testsPassed = currentBuild.result == 'SUCCESS'
                def notificationMessage = """
                    TMDB QA Automation: ${currentBuild.result}
                    Duration: ${currentBuild.durationString}
                    Details: ${BUILD_URL}
                """
                
                // Slack notification
                slackSend(
                    color: testsPassed ? 'good' : 'danger',
                    message: notificationMessage,
                    channel: '#qa-automation'
                )
            }
        }
        failure {
            script {
                // Create issue for failures
                githubNotify(
                    context: 'TMDB QA Tests',
                    description: 'Tests failed',
                    status: 'FAILURE'
                )
            }
        }
    }
}
```

---

## Configuration Management

### Environment-Specific Settings

```bash
# .env.staging
BASE_URL=https://tmdb-discover-staging.surge.sh/
HEADLESS=true
TIMEOUT=5000
RECORD_VIDEO=false

# .env.production
BASE_URL=https://tmdb-discover.surge.sh/
HEADLESS=true
TIMEOUT=3000
RECORD_VIDEO=false

# .env.local (for development)
BASE_URL=http://localhost:3000
HEADLESS=false
TIMEOUT=10000
RECORD_VIDEO=true
```

### Command Examples

```bash
# Run smoke tests only
dotnet test --filter "Category=Smoke"

# Run specific test category
dotnet test --filter "Category=CategoryFilters"

# Run with logging
dotnet test --logger "console;verbosity=detailed"

# Run in parallel with custom settings
dotnet test --parallel --maxParallelWorkers 4

# Generate HTML report
dotnet test --logger "html;LogFileName=report.html"

# Run with specific configuration
dotnet test --configuration Release -p:ENVIRONMENT=staging

# Run tests with timeout
dotnet test --blame-timeout 300000
```

---

## Reporting & Notifications

### Report Types

1. **HTML Report**
   - Auto-generated test summary
   - Visual pass/fail indicators
   - Category breakdown
   - Failure details with screenshots
   - Environment information

2. **Console Report**
   - Real-time test execution status
   - Color-coded results
   - Step-by-step logging

3. **Log Files**
   - Detailed test logs
   - API call information
   - Error traces
   - Screenshots/videos (on failure)

### Artifact Management

```
Reports/
├── TestReport_20240416_120000.html
├── FullTestResults.xml
└── Screenshots/
    ├── TC-001_failure.png
    ├── TC-005_failure.png
    └── ...

Logs/
├── test_log_20240416_120000.log
└── ...
```

### Notifications

**Slack/Teams Integration:**
```
✅ TMDB QA Tests Passed
━━━━━━━━━━━━━━━━━━━━━━━
📊 Results:
  • Total: 43 tests
  • Passed: 43 ✓
  • Failed: 0
  • Pass Rate: 100%

⏱️ Duration: 8m 45s
🔗 Report: [View Report]
━━━━━━━━━━━━━━━━━━━━━━━
```

**Email Summary:**
- Sent to stakeholders after test execution
- Includes pass/fail summary
- Links to reports
- Failure details

---

## Test Failure Handling

### Automatic Issue Creation

When tests fail:

1. **Create GitHub Issue** (if integrated)
   ```
   Title: [AUTOMATION] TC-001 Failed: Filter by Popular
   Labels: [bug, automation, critical]
   Body: Includes failure details, screenshot, logs
   ```

2. **Create Jira Ticket** (if integrated)
   - Severity based on test type
   - Assignee based on component
   - Links to build report

3. **Slack Alert**
   - Immediate notification
   - Failure details
   - Actionable information

### Failure Analysis

```
Failure Pattern Detection:
- If same test fails > 3 times → Mark as flaky
- If multiple tests fail in category → Mark category as broken
- If infrastructure error → Retry pipeline
```

---

## Performance & Load Optimization

### Test Execution Groups

```
Group 1 - Smoke Tests (5-10 minutes)
  └─ Minimal critical path tests
  └─ Fast execution
  └─ Gate for further testing

Group 2 - Core Functionality (15-20 minutes)
  ├─ Category filters
  ├─ Type filters
  ├─ Search
  └─ Pagination

Group 3 - Extended Tests (20-30 minutes)
  ├─ Combined filters
  ├─ Negative tests
  ├─ UI validation
  └─ Performance tests

Group 4 - Optional Tests (Daily only)
  ├─ Regression tests
  ├─ Edge cases
  └─ Load tests
```

### Parallel Execution Strategy

```
PR Validation (All Groups):
├─ Smoke Tests (parallel, 5 machines)
└─ Full Suite (parallel, 8 machines)
   Total time: ~10 minutes

Nightly Run (Extended):
├─ Full Suite (parallel, 16 machines)
├─ Regression Tests (parallel, 8 machines)
└─ Load Tests (sequential, 1 machine)
   Total time: ~30 minutes
```

---

## Deployment Gates

### Success Criteria

```
✓ All Smoke tests PASS
✓ No Critical or High severity failures
✓ Category filter tests PASS
✓ Pagination tests PASS
✓ Pass rate >= 95%
```

### Failure Handling

```
If Smoke Tests FAIL:
  → BLOCK deployment immediately
  → Create P0 incident
  → Notify on-call engineer

If Extended Tests FAIL:
  → Manual approval required
  → Create issues for failures
  → Optional: Proceed if critical path clear

If Flaky Tests Detected:
  → Investigate and fix
  → Re-run before approval
```

---

## Monitoring & Maintenance

### Metrics to Track

```
1. Test Execution
   - Total runtime trend
   - Parallel efficiency
   - Resource usage

2. Test Quality
   - Pass rate over time
   - Flaky test percentage
   - Coverage metrics

3. Failure Analysis
   - Most common failures
   - Failure patterns
   - Environment issues
```

### Health Checks

**Weekly Review:**
- Test execution time trends
- Failure patterns
- Flaky test analysis
- Coverage assessment

**Monthly Review:**
- Identify redundant tests
- Add new test cases for new features
- Update test data
- Performance optimization

---

## Infrastructure Requirements

### Runners/Agents

```
GitHub Actions:
  - ubuntu-latest (free tier or paid)
  - windows-latest (for specific tests)
  - 4GB RAM minimum per runner

Azure Pipelines:
  - Microsoft-hosted: Ubuntu 20.04
  - Self-hosted: Windows Server 2019+ or Ubuntu 20.04+
  - 8GB RAM, SSD recommended

Jenkins:
  - Master: 2GB RAM minimum
  - Agents: 4GB RAM per agent
  - Shared storage for reports
```

### Dependencies

```
Software:
  - .NET 10.0 SDK
  - Chromium browser
  - Node.js (for Playwright CLI)

Credentials:
  - Test environment access (if applicable)
  - Reporting service credentials
  - Notification service tokens
```

---

## Cost Optimization

### For GitHub Actions

```
- Use matrix strategies for parallel execution
- Cache dependencies to reduce restore time
- Use on:schedule for off-peak runs (cheaper)
- Archive only necessary artifacts
- Delete old reports regularly
```

### For Azure Pipelines

```
- Use parallelization for faster execution
- Shared pools for cost distribution
- Reduce pipeline duration to lower costs
- Archive old artifacts automatically
```

### For Jenkins

```
- Implement container-based agents for scaling
- Use spot instances for cost optimization
- Auto-scaling based on queue depth
- Regular cleanup of workspace
```

---

## Example: Complete GitHub Actions Workflow

```yaml
name: TMDB QA Complete Workflow

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]
  schedule:
    - cron: '0 2 * * *'  # Daily 2 AM UTC

jobs:
  smoke-tests:
    runs-on: ubuntu-latest
    name: Smoke Tests
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      - run: |
          dotnet tool install --global Microsoft.Playwright.CLI
          playwright install chromium
      - run: dotnet restore
      - run: dotnet test --filter "Category=Smoke" --logger "html;LogFileName=report-smoke.html"
      - uses: actions/upload-artifact@v3
        if: always()
        with:
          name: smoke-report
          path: report-smoke.html

  full-tests:
    needs: smoke-tests
    runs-on: ubuntu-latest
    name: Full Test Suite
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
      - run: |
          dotnet tool install --global Microsoft.Playwright.CLI
          playwright install chromium
      - run: dotnet restore
      - run: dotnet test --logger "html;LogFileName=report-full.html" --logger "trx;LogFileName=results.trx"
      - uses: actions/upload-artifact@v3
        if: always()
        with:
          name: full-report
          path: |
            report-full.html
            results.trx
      - uses: actions/upload-artifact@v3
        if: always()
        with:
          name: logs
          path: Logs/

  publish-reports:
    needs: full-tests
    runs-on: ubuntu-latest
    if: always()
    steps:
      - uses: actions/download-artifact@v3
      - uses: actions/upload-pages-artifact@v2
        with:
          path: 'full-report'
      - uses: actions/deploy-pages@v1

  notify:
    needs: full-tests
    runs-on: ubuntu-latest
    if: always()
    steps:
      - name: Slack Notification
        uses: slackapi/slack-github-action@v1
        with:
          payload: |
            {
              "text": "TMDB QA Tests: ${{ needs.full-tests.result }}",
              "blocks": [
                {
                  "type": "section",
                  "text": {
                    "type": "mrkdwn",
                    "text": "*TMDB QA Automation*\nStatus: ${{ needs.full-tests.result }}\n${{ github.server_url }}/${{ github.repository }}/actions/runs/${{ github.run_id }}"
                  }
                }
              ]
            }
        env:
          SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK_URL }}
```

---

## Troubleshooting

### Common Issues & Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| Browser not found | Playwright not installed | Run `playwright install chromium` |
| Tests timeout | Slow environment | Increase timeout in config |
| Flaky tests | Network/timing issues | Add retry logic or increase waits |
| Report not generating | Logger misconfigured | Check logger format in command |
| Parallel execution fails | Resource limits | Reduce parallel workers |

---

## Conclusion

This CI/CD integration approach provides:

✅ Automated test execution on every commit  
✅ Parallel execution for faster feedback  
✅ Comprehensive reporting and notifications  
✅ Automatic failure detection and issue creation  
✅ Performance metrics and trend analysis  
✅ Flexible deployment gates based on test results  

Implementation timeline: 1-2 weeks for full setup.
