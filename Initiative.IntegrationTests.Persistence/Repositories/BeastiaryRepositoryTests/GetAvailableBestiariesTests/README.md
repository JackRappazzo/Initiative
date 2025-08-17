# GetAvailableBestiaries Integration Tests

This directory contains comprehensive integration tests for the `GetAvailableBestiaries` method in the `BestiaryRepository` class.

## Test Coverage

The integration tests cover the following scenarios:

### 1. **GivenUserHasNoBestiariesButSystemBestiariesExist**
- **Scenario**: User has no personal bestiaries, but system bestiaries exist
- **Expected Result**: Returns only system bestiaries
- **Validates**: 
  - Only system bestiaries are returned
  - Correct bestiary data (ID, Name, OwnerId = null)
  - Proper count of results

### 2. **GivenUserHasOwnBestiariesAndSystemBestiariesExist**
- **Scenario**: User owns personal bestiaries and system bestiaries exist
- **Expected Result**: Returns both user's bestiaries and system bestiaries
- **Validates**:
  - Both user and system bestiaries are returned
  - Correct separation of user vs system bestiaries
  - Accurate bestiary data for all returned items

### 3. **GivenNoBestiariesExist**
- **Scenario**: No bestiaries exist in the database
- **Expected Result**: Returns empty collection
- **Validates**:
  - Method doesn't throw exceptions
  - Returns empty but non-null collection

### 4. **GivenOtherUsersHaveBestiariesButCurrentUserDoesNot**
- **Scenario**: Other users have bestiaries, but current user doesn't
- **Expected Result**: Returns only system bestiaries, excludes other users' bestiaries
- **Validates**:
  - Privacy: Other users' bestiaries are not returned
  - Only system bestiaries are accessible
  - Proper isolation of user data

### 5. **GivenNullUserId**
- **Scenario**: Method called with null userId
- **Expected Result**: Returns only system bestiaries
- **Validates**:
  - Graceful handling of null user ID
  - No user bestiaries returned when userId is null
  - System bestiaries still accessible

### 6. **GivenEmptyStringUserId**
- **Scenario**: Method called with empty string userId
- **Expected Result**: Returns only system bestiaries
- **Validates**:
  - Graceful handling of empty string user ID
  - Same behavior as null userId
  - System bestiaries accessibility maintained

### 7. **GivenComplexScenarioWithMultipleUsersAndBestiaries**
- **Scenario**: Complex scenario with multiple users, each having multiple bestiaries, plus system bestiaries
- **Expected Result**: Returns only the target user's bestiaries plus all system bestiaries
- **Validates**:
  - Comprehensive scenario testing
  - Proper user isolation
  - Correct handling of multiple bestiaries per user
  - System bestiary availability to all users

## Technical Details

### Database Isolation
- Each test runs in isolation using the MongoDB test database
- Tests inherit from `WhenTestingBeastiaryRepository` which provides database setup
- Uses `CancellationToken.None` for synchronous operation testing

### Test Framework
- Uses **LeapingGorilla.Testing** framework with **NUnit** attributes
- Follows BDD (Behavior Driven Development) pattern with Given/When/Then structure
- Each test is composable and self-contained

### Data Validation
Tests validate:
- **Correctness**: Right bestiaries are returned
- **Security**: User isolation (users can't see other users' private bestiaries)
- **Completeness**: All expected bestiaries are included
- **Data Integrity**: Returned DTOs contain correct data (ID, Name, OwnerId)

## Running the Tests

These integration tests require:
1. MongoDB test database connection
2. NUnit test runner
3. LeapingGorilla.Testing framework

Execute via your preferred test runner (Visual Studio Test Explorer, dotnet test, etc.)

## Method Signature

```csharp
Task<IEnumerable<GetAvailableBestiaryDto>> GetAvailableBestiaries(string userId, CancellationToken cancellationToken)
```

The method returns bestiaries available to a specific user:
- **System Bestiaries**: Always available to all users (OwnerId = null)
- **User Bestiaries**: Only the specified user's bestiaries (OwnerId = userId)
- **Other Users' Bestiaries**: Never returned (maintains privacy)