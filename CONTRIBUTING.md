# Contributing to UI for Universal Windows Platform

## Before You Start

Anyone wishing to contribute to the UI for Universal Windows Platform project MUST read & sign the [UI For Universal Windows Platform Contribution License Agreement](https://docs.google.com/forms/d/e/1FAIpQLSfQAzVxnnfwRQmtJCVmB41_ig1gYow--Gr8qLvaDxJRNHPtUQ/viewform). The UI for UWP team cannot accept pull requests from users who have not signed the CLA first.

## Introduction

These guidelines are here to facilitate your contribution and streamline the process of getting changes merged into this project and released. Any contributions you can make will help tremendously, even if only in the form of an issue report. Following these guidelines will help to streamline the pull request and change submission process.

## Reporting Bugs

1. Always update to the most recent master release; the bug may already be resolved.
2. Search for similar issues in the issues list for this repo; it may already be an identified problem.
3. Make sure you can reproduce your problem locally in an isolated project with no external dependencies.
4. If this is a bug or problem that is clear, simple, and is unlikely to require any discussion -- it is OK to open an issue on GitHub with a reproduction of the bug including workflows, screenshots, links to examples. If you'd rather take matters into your own hands, fix the bug yourself (jump down to the "Code Fixes and Enhancements" section).

## Requesting New Features

Do not submit a feature request on GitHub; all feature requests on GitHub will be closed. Instead, visit the [UI for UWP Feedback portal](https://feedback.telerik.com/Project/167), and search this list for similar feature requests.

## Asking for Help

The UI for UWP team does *not* provide formal support for the product, except to those customers who have purchased a [commercial license for UI for UWP](http://www.telerik.com/universal-windows-platform-ui) or a support-only package from Telerik.com. Please do not create support requests for this project in the issues list for this repo, as these will be immediately closed and you'll be directed to post your question on a community forum.

## Code Fixes and Enhancements

### 1. Log an Issue

Before doing anything else, we ask that you file an issue in the Issues list for this project. First, be sure to check the list to ensure that your issue hasn't already been logged. If you're free and clear, file an issue and provide a detailed description of the bug or feature you're interested in. If you're also planning to work on the issue you're creating, let us know so that we can help and provide feedback.

### 2. Fork and Branch

#### Fork Us, Then Create A Topic Branch For Your Work

The work you are doing for your pull request should not be done in the master branch of your forked repository. Create a topic branch for your work. This allows you to isolate the work you are doing from other changes that may be happening.

Github is a smart system, too. If you submit a pull request from a topic branch and we ask you to fix something, pushing a change to your topic branch will automatically update the pull request.

#### Isolate Your Changes For The Pull Request

See the previous item on creating a topic branch.

If you don't use a topic branch, we may ask you to re-do your pull request on a topic branch. If your pull request contains commits or other changes that are not related to the pull request, we will ask you to re-do your pull request.

#### Branch from "master"

Always create a branch for your work from the "master" branch.
This will facilitate easier pull request management for the continuous work that is done in the master branch.


#### (optional) Squash your commits

When you've completed your work on a topic branch, you may squash your work down into fewer commits to make the merge process easier. For information on squashing via an interactive rebase, see [the rebase documentation on GitHub](https://help.github.com/articles/interactive-rebase)

### 3. Submit a Pull Request

See [Github's documentation for pull requests](https://help.github.com/articles/using-pull-requests).

Pull requests are the preferred way to contribute to UI for UWP. Any time you can send us a pull request with the changes that you want, we will have an easier time seeing what you are trying to do. But a pull request itself is not usually sufficient. There needs to be some context and purpose with it, and it should be done against specific branch.

### Provide A Meaningful Description

It is very important to provide a meaningful description with your pull requests that alter any code. A good format for these descriptions will include:

1. Why: The problem you are facing (in as much detail as is necessary to describe the problem to someone who doesn't know anything about the system you're building)

2. What: A summary of the proposed solution

3. How: A description of how this solution solves the problem, in more detail than item #2

4. Any additional discussion on possible problems this might introduce, questions that you have related to the changes, etc.

Without at least the first 2 items in this list, we won't have any clue why you're changing the code. The first thing we'll ask, then, is that you add that information.

## Code Style

All code contributed to this project should adhere to a consistent style, so please keep these in mind before you submit your Pull Requests:

- Space indentation, size of 4
- Do use braces for one-line blocks (if, for, while, do)
- Place braces, "else", "catch", and "finally" on new line
- Qualify member access with this
