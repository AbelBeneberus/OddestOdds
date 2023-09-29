Feature: Back Office Odds Management API
As an Odds Handler,
I want an API to manage odds
So that I can effectively control the displayed odds programmatically.

    Background:
        Given a Fixture with its associated Market is already created
 
    Scenario: Create new odds via API
        Given I have the details for a new odd
        When I send a POST request to the 'Create Odd' endpoint
        Then the response should confirm the odds have been created

    Scenario: Update existing odds via API
        Given I have an existing odd ID and updated details
        When I send a PUT request to the 'Update Odds' endpoint
        Then the response should confirm the odds have been updated

    Scenario: Delete existing odds via API
        Given I have an existing odd ID
        When I send a DELETE request to the 'Delete Odds' endpoint
        Then the response should confirm the odd has been deleted

    Scenario: Publish existing odds via API
        Given I have an existing odd ID
        And I have pushAllOdd Request
        And the SignalR client is ready
        When I send a POST request to the 'Publish Odds' endpoint
        Then the response should confirm the odd has been published
        And I should receive the updated odd via SignalR