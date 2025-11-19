Feature: GetUserById - Retrieve specific user by ID

  Background:
    Given I have the "UserAPI" service initialized

  Scenario Outline: GetUserById endpoint tests
    When I create a GET request to "GetUserById" endpoint with path params "<pathParams>" test case "<tcNo>" query params "<queryParams>" headers "<headers>"
    Then I receive a response with HTTP status code "<statusCode>" with status text "<statusText>"
    And I verify that the field "<fieldName>" in the response is "<fieldValue>"
    And I verify the "<fieldToContain>" from the response to contain "<containsValue>"
    And I verify the headers "<expectedHeaders>" to be present
    And I verify the header value for the key "<headerKey>" to be "<headerValue>"
    And I verify the "<fieldExists>" in the response "exists"
    And I verify the "<fieldNotExists>" in the response "does not exist"
    And I verify the "<fieldType>" type in the response to be a "<expectedType>"
    And the response returned should match the schema "<schemaFile>"

    Examples:
      | tcNo  | description                      | pathParams | queryParams | headers | statusCode | statusText | fieldName       | fieldValue | fieldToContain  | containsValue | expectedHeaders | headerKey    | headerValue      | fieldExists     | fieldNotExists | fieldType   | expectedType | schemaFile                  |
      | UG001 | Get user 2 - verify ID           | id=2       | none        | none    | 200        | OK         | data.id         | 2          | data.email      | @reqres.in    | Content-Type    | Content-Type | application/json | data            | nonexistent    | data.id     | integer      | get-user-by-id-schema.json  |
      | UG002 | Get user 3 - verify ID           | id=3       | none        | none    | 200        | OK         | data.id         | 3          | data.email      | @reqres.in    | Content-Type    | Content-Type | application/json | data.id         | invalid_field  | data.id     | integer      | get-user-by-id-schema.json  |
      | UG003 | Get user 4 - verify ID           | id=4       | none        | none    | 200        | OK         | data.id         | 4          | data.first_name | e             | Content-Type    | Content-Type | application/json | data.avatar     | temp_field     | data.id     | integer      | get-user-by-id-schema.json  |
      | UG004 | Get user 2 - verify email exists | id=2       | none        | none    | 200        | OK         | data.email      | NotEmpty   | data.last_name  | Weaver        | Content-Type    | Content-Type | application/json | data.email      | n/a            | data.email  | string       | get-user-by-id-schema.json  |
      | UG005 | Get user 2 - verify first name   | id=2       | none        | none    | 200        | OK         | data.first_name | Janet      | n/a             | n/a           | Content-Type    | Content-Type | application/json | data.first_name | n/a            | data.avatar | string       | get-user-by-id-schema.json  |
      | UG006 | Get user 3 - verify first name   | id=3       | none        | none    | 200        | OK         | data.first_name | Emma       | data.avatar     | https://      | Content-Type    | Content-Type | application/json | support         | n/a            | support     | object       | get-user-by-id-schema.json  |
      | UG007 | Get user 999 - not found         | id=999     | none        | none    | 404        | Not Found  | n/a             | n/a        | n/a             | n/a           | n/a             | n/a          | n/a              | n/a             | n/a            | n/a         | n/a          | none                        |
