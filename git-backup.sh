#!/bin/bash

# Git Backup Script
# Automatically stages and commits changes with a timestamped message

set -e  # Exit on error

echo "=========================================="
echo "Git Backup Script"
echo "=========================================="
echo ""

# Check if we're in a git repository
if ! git rev-parse --git-dir > /dev/null 2>&1; then
    echo "Error: Not a git repository!"
    exit 1
fi

# Show current status
echo "Current Git Status:"
echo "==================="
git status --short
echo ""

# Check if there are any changes
if git diff-index --quiet HEAD --; then
    echo "No changes to commit."
    exit 0
fi

# Get commit message from user or use default
if [ -z "$1" ]; then
    TIMESTAMP=$(date "+%Y-%m-%d %H:%M:%S")
    COMMIT_MSG="Backup: $TIMESTAMP"
    echo "No commit message provided. Using default: '$COMMIT_MSG'"
else
    COMMIT_MSG="$1"
    echo "Using commit message: '$COMMIT_MSG'"
fi
echo ""

# Stage important files (exclude build artifacts and IDE files)
echo "Staging files..."
echo "================"

# Add source code files
git add SampleWebApp/ 2>/dev/null || true
git add SampleWebApp.Tests/*.cs 2>/dev/null || true
git add SampleWebApp.Tests/BDDTests/ 2>/dev/null || true
git add SampleWebApp.Tests/UITests/ 2>/dev/null || true
git add SampleWebApp.Tests/UnitTests/ 2>/dev/null || true
git add SampleWebApp.Tests/*.csproj 2>/dev/null || true
git add SampleWebApp.Tests/*.json 2>/dev/null || true
git add SampleWebApp.Tests/.runsettings 2>/dev/null || true

# Add scripts
git add *.sh 2>/dev/null || true
git add *.ps1 2>/dev/null || true

# Add project files
git add *.sln 2>/dev/null || true
git add README.md 2>/dev/null || true
git add .gitignore 2>/dev/null || true

echo "Files staged successfully."
echo ""

# Show what will be committed
echo "Files to be committed:"
echo "======================"
git diff --cached --name-status
echo ""

# Confirm before committing
read -p "Do you want to commit these changes? (y/n): " -n 1 -r
echo ""

if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Commit cancelled."
    git reset > /dev/null 2>&1
    exit 0
fi

# Create commit
echo ""
echo "Creating commit..."
git commit -m "$(cat <<EOF
$COMMIT_MSG

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
EOF
)"

echo ""
echo "✓ Commit created successfully!"
echo ""

# Show the commit
echo "Latest commit:"
echo "==============="
git log -1 --oneline
echo ""

# Ask if user wants to push
read -p "Do you want to push to remote? (y/n): " -n 1 -r
echo ""

if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo ""
    echo "Pushing to remote..."
    BRANCH=$(git branch --show-current)
    git push origin "$BRANCH"
    echo ""
    echo "✓ Changes pushed to origin/$BRANCH"
else
    echo "Changes committed locally. Run 'git push' when ready to push."
fi

echo ""
echo "=========================================="
echo "Backup complete!"
echo "=========================================="
