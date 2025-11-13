Feature: Todo Management
    As a user
    I want to manage my todo list
    So that I can track my tasks effectively

Background:
    Given the todo application is loaded
    And the todo list is empty

@smoke @critical
Scenario: Add a new todo item
    When I add a todo with text "Buy groceries"
    Then the todo "Buy groceries" should be displayed in the list
    And the remaining tasks count should be 1

@critical
Scenario: Complete a todo item
    Given I have added a todo "Complete project report"
    When I mark the todo "Complete project report" as completed
    Then the todo "Complete project report" should be marked as completed
    And the remaining tasks count should be 0

@critical
Scenario: Delete a todo item
    Given I have added a todo "Task to delete"
    When I delete the todo "Task to delete"
    Then the todo "Task to delete" should not be in the list

@filter
Scenario: Filter active todos
    Given I have added the following todos:
        | Text             | Completed |
        | Active task 1    | false     |
        | Completed task   | true      |
        | Active task 2    | false     |
    When I filter by "Active"
    Then I should see 2 todos in the list
    And all displayed todos should be active

@filter
Scenario: Filter completed todos
    Given I have added the following todos:
        | Text             | Completed |
        | Active task      | false     |
        | Completed task 1 | true      |
        | Completed task 2 | true      |
    When I filter by "Completed"
    Then I should see 2 todos in the list
    And all displayed todos should be completed

@clear
Scenario: Clear completed todos
    Given I have added the following todos:
        | Text             | Completed |
        | Active task      | false     |
        | Completed task 1 | true      |
        | Completed task 2 | true      |
    When I clear completed todos
    Then I should see 1 todos in the list
    And all displayed todos should be active

@validation
Scenario Outline: Add todo validation
    When I add a todo with text "<TodoText>"
    Then the todo count should be <ExpectedCount>

    Examples:
        | TodoText | ExpectedCount |
        |          | 0             |
        |          | 0             |
        | Valid    | 1             |