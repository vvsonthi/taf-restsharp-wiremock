Feature: PartialUpdateUser - Partial update of user fields

  Background:
    Given I have the "UserAPI" service initialized

  Scenario Outline: PartialUpdateUser endpoint tests
    When I create a PATCH request to "PartialUpdateUser" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>" with payload <payload>
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"
    And I verify that the field "<fieldName>" in the response is "<fieldValue>"

    Examples:
      | tcNo  | description                        | pathParams | queryParams | headers | payload                 | statusCode | statusText | fieldName | fieldValue  |
      | UH001 | Partial update user 2 - job field  | id=2       | none        | none    | patch-user-payload.json | 200        | OK         | job       | team leader |
      | UH002 | Partial update user 2 - verify timestamp | id=2 | none        | none    | patch-user-payload.json | 200        | OK         | updatedAt | NotEmpty    |
      | UH003 | Partial update user 3 - job field  | id=3       | none        | none    | patch-user-payload.json | 200        | OK         | job       | team leader |
