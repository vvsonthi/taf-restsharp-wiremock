Feature: Response Schema Validation

  Scenario: Validate single user response schema
    When I GET user with id 2 from endpoint '/api/users/{id}'
    Then the GET response should have status 200
    And the response should have valid single user schema with all required fields
    And user email should match email pattern
    And user id should be within valid range

  Scenario: Validate user list response schema
    When I GET all users from endpoint '/api/users'
    Then the GET response should have status 200
    And the response should have valid user list schema with pagination
    And all users in list should have valid schema
    And pagination fields should be within valid range

  Scenario: Validate create user response schema
    When I POST the payload from file 'create-user-payload.json' to endpoint '/api/users'
    Then the POST response should have status 201
    And the response should have valid create user schema
    And createdAt should be in ISO 8601 format
    And created user id should not be empty

  Scenario: Validate update user response schema
    When I PUT the payload from file 'update-user-payload.json' to endpoint '/api/users/2'
    Then the PUT response should have status 200
    And the response should have valid update user schema
    And updatedAt should be in ISO 8601 format

  Scenario: Validate required fields exist in response
    When I GET user with id 2 from endpoint '/api/users/{id}'
    Then the GET response should have status 200
    And the response should contain all required fields: id, email, first_name, last_name, avatar

  Scenario: Validate field types in response
    When I GET user with id 2 from endpoint '/api/users/{id}'
    Then the GET response should have status 200
    And field 'id' should be of type integer
    And field 'email' should be of type string
    And field 'first_name' should be of type string
    And field 'last_name' should be of type string
    And field 'avatar' should be of type string
