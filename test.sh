#!/bin/bash

#############################################
# AutoInsight API - CRUD Complete Test Suite
# Tests all resources and operations
#############################################

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
MAGENTA='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Configuration
API_IP="${API_IP:-localhost}"
API_PORT="${API_PORT:-5100}"
BASE_URL="http://$API_IP:$API_PORT"

# Test counters
TESTS_RUN=0
TESTS_PASSED=0
TESTS_FAILED=0

# Output helpers
print_header() {
    echo ""
    echo -e "${CYAN}========================================${NC}"
    echo -e "${CYAN}$1${NC}"
    echo -e "${CYAN}========================================${NC}"
}

print_test() {
    echo -e "${YELLOW}▶ TEST: $1${NC}"
    TESTS_RUN=$((TESTS_RUN + 1))
}

print_success() {
    echo -e "${GREEN}✓ SUCCESS: $1${NC}"
    TESTS_PASSED=$((TESTS_PASSED + 1))
}

print_error() {
    echo -e "${RED}✗ FAILED: $1${NC}"
    TESTS_FAILED=$((TESTS_FAILED + 1))
}

print_info() {
    echo -e "${BLUE}ℹ INFO: $1${NC}"
}

print_response() {
    echo -e "${MAGENTA}Response: $1${NC}"
}

# Check if jq is installed
if ! command -v jq &> /dev/null; then
    echo -e "${RED}Error: jq is required but not installed. Install with: brew install jq${NC}"
    exit 1
fi

# Test API availability
print_header "CHECKING API AVAILABILITY"
print_test "Health check"
HEALTH_RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/health")
HTTP_CODE=$(echo "$HEALTH_RESPONSE" | tail -n 1)
if [ "$HTTP_CODE" -eq 200 ]; then
    print_success "API is healthy and running at $BASE_URL"
else
    print_error "API is not responding correctly (HTTP $HTTP_CODE)"
    exit 1
fi

#############################################
# YARDS - CRUD COMPLETE
#############################################

print_header "TESTING YARDS RESOURCE"

# CREATE YARD
print_test "Create new yard (POST /yards)"
CREATE_YARD_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/yards" \
  -H "Content-Type: application/json" \
  -d '{
    "ownerId": "owner_test_001",
    "address": {
      "country": "BR",
      "state": "SP",
      "city": "São Paulo",
      "zipCode": "01311-000",
      "neighborhood": "Bela Vista",
      "complement": "Av. Paulista, 1106 - Teste Automatizado"
    }
  }')

HTTP_CODE=$(echo "$CREATE_YARD_RESPONSE" | tail -n 1)
RESPONSE_BODY=$(echo "$CREATE_YARD_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" -eq 201 ]; then
    YARD_ID=$(echo "$RESPONSE_BODY" | jq -r '.id')
    print_success "Yard created successfully with ID: $YARD_ID"
    print_response "$RESPONSE_BODY" | jq '.'
else
    print_error "Failed to create yard (HTTP $HTTP_CODE)"
    print_response "$RESPONSE_BODY"
    exit 1
fi

# READ YARD BY ID
print_test "Get yard by ID (GET /yards/$YARD_ID)"
GET_YARD_RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/yards/$YARD_ID")
HTTP_CODE=$(echo "$GET_YARD_RESPONSE" | tail -n 1)
RESPONSE_BODY=$(echo "$GET_YARD_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" -eq 200 ]; then
    print_success "Yard retrieved successfully"
    print_response "$RESPONSE_BODY" | jq '.'
else
    print_error "Failed to get yard (HTTP $HTTP_CODE)"
fi

# LIST YARDS
print_test "List yards with pagination (GET /yards?pageNumber=1&pageSize=5)"
LIST_YARDS_RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/yards?pageNumber=1&pageSize=5")
HTTP_CODE=$(echo "$LIST_YARDS_RESPONSE" | tail -n 1)
RESPONSE_BODY=$(echo "$LIST_YARDS_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" -eq 200 ]; then
    TOTAL_RECORDS=$(echo "$RESPONSE_BODY" | jq -r '.totalRecords')
    print_success "Retrieved yards list (Total: $TOTAL_RECORDS records)"
    print_response "$RESPONSE_BODY" | jq '.'
else
    print_error "Failed to list yards (HTTP $HTTP_CODE)"
fi

# UPDATE YARD
print_test "Update yard (PATCH /yards/$YARD_ID)"
UPDATE_YARD_RESPONSE=$(curl -s -w "\n%{http_code}" -X PATCH "$BASE_URL/yards/$YARD_ID" \
  -H "Content-Type: application/json" \
  -d '{
    "ownerId": "owner_test_001",
    "address": {
      "country": "BR",
      "state": "SP",
      "city": "São Paulo",
      "zipCode": "01311-000",
      "neighborhood": "Jardins",
      "complement": "Av. Paulista, 2000 - ATUALIZADO"
    }
  }')

HTTP_CODE=$(echo "$UPDATE_YARD_RESPONSE" | tail -n 1)
RESPONSE_BODY=$(echo "$UPDATE_YARD_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" -eq 200 ]; then
    print_success "Yard updated successfully"
    print_response "$RESPONSE_BODY" | jq '.'
else
    print_error "Failed to update yard (HTTP $HTTP_CODE)"
fi

#############################################
# YARD EMPLOYEES - CRUD COMPLETE
#############################################

print_header "TESTING YARD EMPLOYEES RESOURCE"

# CREATE YARD EMPLOYEE (via Invite)
print_test "Create employee invite (POST /yards/$YARD_ID/invites)"
CREATE_INVITE_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/yards/$YARD_ID/invites" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva",
    "email": "joao.silva@test.com",
    "role": "MEMBER"
  }')

HTTP_CODE=$(echo "$CREATE_INVITE_RESPONSE" | tail -n 1)
RESPONSE_BODY=$(echo "$CREATE_INVITE_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" -eq 201 ]; then
    INVITE_TOKEN=$(echo "$RESPONSE_BODY" | jq -r '.token')
    INVITE_ID=$(echo "$RESPONSE_BODY" | jq -r '.id')
    print_success "Invite created with token: $INVITE_TOKEN"
    print_response "$RESPONSE_BODY" | jq '.'
else
    print_error "Failed to create invite (HTTP $HTTP_CODE)"
    print_response "$RESPONSE_BODY"
fi

# ACCEPT INVITE (Creates Employee)
if [ ! -z "$INVITE_TOKEN" ]; then
    print_test "Accept invite (POST /invites/$INVITE_TOKEN/accept)"
    ACCEPT_INVITE_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/invites/$INVITE_TOKEN/accept" \
      -H "Content-Type: application/json" \
      -d '{
        "userId": "user_test_001",
        "imageUrl": "https://example.com/photo.jpg"
      }')

    HTTP_CODE=$(echo "$ACCEPT_INVITE_RESPONSE" | tail -n 1)
    RESPONSE_BODY=$(echo "$ACCEPT_INVITE_RESPONSE" | sed '$d')

    if [ "$HTTP_CODE" -eq 200 ]; then
        EMPLOYEE_ID=$(echo "$RESPONSE_BODY" | jq -r '.id')
        print_success "Invite accepted, employee created with ID: $EMPLOYEE_ID"
        print_response "$RESPONSE_BODY" | jq '.'
    else
        print_error "Failed to accept invite (HTTP $HTTP_CODE)"
        print_response "$RESPONSE_BODY"
    fi
fi

# LIST YARD EMPLOYEES
print_test "List yard employees (GET /yards/$YARD_ID/employees)"
LIST_EMPLOYEES_RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/yards/$YARD_ID/employees?pageNumber=1&pageSize=10")
HTTP_CODE=$(echo "$LIST_EMPLOYEES_RESPONSE" | tail -n 1)
RESPONSE_BODY=$(echo "$LIST_EMPLOYEES_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" -eq 200 ]; then
    TOTAL_EMPLOYEES=$(echo "$RESPONSE_BODY" | jq -r '.totalRecords')
    print_success "Retrieved employees list (Total: $TOTAL_EMPLOYEES employees)"
    print_response "$RESPONSE_BODY" | jq '.'
else
    print_error "Failed to list employees (HTTP $HTTP_CODE)"
fi

# GET EMPLOYEE BY ID
if [ ! -z "$EMPLOYEE_ID" ]; then
    print_test "Get employee by ID (GET /yards/$YARD_ID/employees/$EMPLOYEE_ID)"
    GET_EMPLOYEE_RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/yards/$YARD_ID/employees/$EMPLOYEE_ID")
    HTTP_CODE=$(echo "$GET_EMPLOYEE_RESPONSE" | tail -n 1)
    RESPONSE_BODY=$(echo "$GET_EMPLOYEE_RESPONSE" | sed '$d')

    if [ "$HTTP_CODE" -eq 200 ]; then
        print_success "Employee retrieved successfully"
        print_response "$RESPONSE_BODY" | jq '.'
    else
        print_error "Failed to get employee (HTTP $HTTP_CODE)"
    fi

    # UPDATE EMPLOYEE
    print_test "Update employee (PATCH /yards/$YARD_ID/employees/$EMPLOYEE_ID)"
    UPDATE_EMPLOYEE_RESPONSE=$(curl -s -w "\n%{http_code}" -X PATCH "$BASE_URL/yards/$YARD_ID/employees/$EMPLOYEE_ID" \
      -H "Content-Type: application/json" \
      -d '{
        "name": "João Silva - Atualizado",
        "imageUrl": "https://example.com/photo-updated.jpg",
        "role": "ADMIN",
        "userId": "user_test_001"
      }')

    HTTP_CODE=$(echo "$UPDATE_EMPLOYEE_RESPONSE" | tail -n 1)
    RESPONSE_BODY=$(echo "$UPDATE_EMPLOYEE_RESPONSE" | sed '$d')

    if [ "$HTTP_CODE" -eq 200 ]; then
        print_success "Employee updated successfully"
        print_response "$RESPONSE_BODY" | jq '.'
    else
        print_error "Failed to update employee (HTTP $HTTP_CODE)"
    fi
fi

#############################################
# INVITES - ADDITIONAL OPERATIONS
#############################################

print_header "TESTING INVITES RESOURCE"

# LIST YARD INVITES
print_test "List yard invites (GET /yards/$YARD_ID/invites)"
LIST_INVITES_RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/yards/$YARD_ID/invites?pageNumber=1&pageSize=10")
HTTP_CODE=$(echo "$LIST_INVITES_RESPONSE" | tail -n 1)
RESPONSE_BODY=$(echo "$LIST_INVITES_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" -eq 200 ]; then
    TOTAL_INVITES=$(echo "$RESPONSE_BODY" | jq -r '.totalRecords')
    print_success "Retrieved invites list (Total: $TOTAL_INVITES invites)"
    print_response "$RESPONSE_BODY" | jq '.'
else
    print_error "Failed to list invites (HTTP $HTTP_CODE)"
fi

# CREATE INVITE FOR REJECTION TEST
print_test "Create invite for rejection test"
CREATE_REJECT_INVITE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/yards/$YARD_ID/invites" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Maria Santos",
    "email": "maria.santos@test.com",
    "role": "MEMBER"
  }')

HTTP_CODE=$(echo "$CREATE_REJECT_INVITE" | tail -n 1)
RESPONSE_BODY=$(echo "$CREATE_REJECT_INVITE" | sed '$d')

if [ "$HTTP_CODE" -eq 201 ]; then
    REJECT_TOKEN=$(echo "$RESPONSE_BODY" | jq -r '.token')
    print_success "Invite created for rejection test"
    
    # REJECT INVITE
    print_test "Reject invite (POST /invites/$REJECT_TOKEN/reject)"
    REJECT_INVITE_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/invites/$REJECT_TOKEN/reject")
    HTTP_CODE=$(echo "$REJECT_INVITE_RESPONSE" | tail -n 1)

    if [ "$HTTP_CODE" -eq 204 ]; then
        print_success "Invite rejected successfully"
    else
        print_error "Failed to reject invite (HTTP $HTTP_CODE)"
    fi
fi

# LIST INVITES BY EMAIL
print_test "List pending invites by email (GET /invites/email/joao.silva@test.com)"
EMAIL_INVITES_RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/invites/email/joao.silva@test.com?pageNumber=1&pageSize=10")
HTTP_CODE=$(echo "$EMAIL_INVITES_RESPONSE" | tail -n 1)
RESPONSE_BODY=$(echo "$EMAIL_INVITES_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" -eq 200 ]; then
    print_success "Retrieved email invites"
    print_response "$RESPONSE_BODY" | jq '.'
else
    print_error "Failed to get email invites (HTTP $HTTP_CODE)"
fi

# LIST INVITES BY USER
if [ ! -z "$EMPLOYEE_ID" ]; then
    print_test "List user invite history (GET /invites/user/user_test_001)"
    USER_INVITES_RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/invites/user/user_test_001?pageNumber=1&pageSize=10")
    HTTP_CODE=$(echo "$USER_INVITES_RESPONSE" | tail -n 1)
    RESPONSE_BODY=$(echo "$USER_INVITES_RESPONSE" | sed '$d')

    if [ "$HTTP_CODE" -eq 200 ]; then
        print_success "Retrieved user invite history"
        print_response "$RESPONSE_BODY" | jq '.'
    else
        print_error "Failed to get user invites (HTTP $HTTP_CODE)"
    fi
fi

#############################################
# YARD VEHICLES - CRUD COMPLETE
#############################################

print_header "TESTING YARD VEHICLES RESOURCE"

# CREATE YARD VEHICLE (with new vehicle and model)
print_test "Create yard vehicle with new vehicle and model (POST /yards/$YARD_ID/vehicles)"
CREATE_YARD_VEHICLE_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/yards/$YARD_ID/vehicles" \
  -H "Content-Type: application/json" \
  -d '{
    "status": "WAITING",
    "enteredAt": "2025-10-01T10:00:00Z",
    "vehicle": {
      "plate": "ABC1D23",
      "model": {
        "name": "Honda CG 160",
        "year": 2023
      },
      "userId": "user_vehicle_001"
    }
  }')

HTTP_CODE=$(echo "$CREATE_YARD_VEHICLE_RESPONSE" | tail -n 1)
RESPONSE_BODY=$(echo "$CREATE_YARD_VEHICLE_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" -eq 201 ]; then
    YARD_VEHICLE_ID=$(echo "$RESPONSE_BODY" | jq -r '.id')
    VEHICLE_ID=$(echo "$RESPONSE_BODY" | jq -r '.vehicle.id')
    MODEL_ID=$(echo "$RESPONSE_BODY" | jq -r '.vehicle.model.id')
    print_success "Yard vehicle created with ID: $YARD_VEHICLE_ID"
    print_info "Vehicle ID: $VEHICLE_ID, Model ID: $MODEL_ID"
    print_response "$RESPONSE_BODY" | jq '.'
else
    print_error "Failed to create yard vehicle (HTTP $HTTP_CODE)"
    print_response "$RESPONSE_BODY"
fi

# LIST YARD VEHICLES
print_test "List yard vehicles (GET /yards/$YARD_ID/vehicles)"
LIST_YARD_VEHICLES_RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/yards/$YARD_ID/vehicles?pageNumber=1&pageSize=10")
HTTP_CODE=$(echo "$LIST_YARD_VEHICLES_RESPONSE" | tail -n 1)
RESPONSE_BODY=$(echo "$LIST_YARD_VEHICLES_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" -eq 200 ]; then
    TOTAL_VEHICLES=$(echo "$RESPONSE_BODY" | jq -r '.totalRecords')
    print_success "Retrieved yard vehicles (Total: $TOTAL_VEHICLES vehicles)"
    print_response "$RESPONSE_BODY" | jq '.'
else
    print_error "Failed to list yard vehicles (HTTP $HTTP_CODE)"
fi

# GET YARD VEHICLE BY ID
if [ ! -z "$YARD_VEHICLE_ID" ]; then
    print_test "Get yard vehicle by ID (GET /yards/$YARD_ID/vehicles/$YARD_VEHICLE_ID)"
    GET_YARD_VEHICLE_RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/yards/$YARD_ID/vehicles/$YARD_VEHICLE_ID")
    HTTP_CODE=$(echo "$GET_YARD_VEHICLE_RESPONSE" | tail -n 1)
    RESPONSE_BODY=$(echo "$GET_YARD_VEHICLE_RESPONSE" | sed '$d')

    if [ "$HTTP_CODE" -eq 200 ]; then
        print_success "Yard vehicle retrieved successfully"
        print_response "$RESPONSE_BODY" | jq '.'
    else
        print_error "Failed to get yard vehicle (HTTP $HTTP_CODE)"
    fi

    # UPDATE YARD VEHICLE (agora envia apenas status, enteredAt e leftAt)
    print_test "Update yard vehicle status (PATCH /yards/$YARD_ID/vehicles/$YARD_VEHICLE_ID)"
    UPDATE_YARD_VEHICLE_RESPONSE=$(curl -s -w "\n%{http_code}" -X PATCH "$BASE_URL/yards/$YARD_ID/vehicles/$YARD_VEHICLE_ID" \
      -H "Content-Type: application/json" \
      -d '{
        "status": "ON_SERVICE",
        "enteredAt": "2025-10-01T10:00:00Z",
        "leftAt": null
      }')

    HTTP_CODE=$(echo "$UPDATE_YARD_VEHICLE_RESPONSE" | tail -n 1)
    RESPONSE_BODY=$(echo "$UPDATE_YARD_VEHICLE_RESPONSE" | sed '$d')

    if [ "$HTTP_CODE" -eq 200 ]; then
        print_success "Yard vehicle updated successfully"
        print_response "$RESPONSE_BODY" | jq '.'
    else
        print_error "Failed to update yard vehicle (HTTP $HTTP_CODE)"
        print_response "$RESPONSE_BODY"
    fi
fi

#############################################
# VEHICLES - READ OPERATIONS
#############################################

print_header "TESTING VEHICLES RESOURCE"

# GET VEHICLE BY ID
if [ ! -z "$VEHICLE_ID" ]; then
    print_test "Get vehicle by ID (GET /vehicles/$VEHICLE_ID)"
    GET_VEHICLE_RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/vehicles/$VEHICLE_ID")
    HTTP_CODE=$(echo "$GET_VEHICLE_RESPONSE" | tail -n 1)
    RESPONSE_BODY=$(echo "$GET_VEHICLE_RESPONSE" | sed '$d')

    if [ "$HTTP_CODE" -eq 200 ]; then
        print_success "Vehicle retrieved successfully"
        print_response "$RESPONSE_BODY" | jq '.'
    else
        print_error "Failed to get vehicle (HTTP $HTTP_CODE)"
    fi
fi

#############################################
# CLEANUP - DELETE OPERATIONS
#############################################

print_header "TESTING DELETE OPERATIONS (CLEANUP)"

# DELETE EMPLOYEE
if [ ! -z "$EMPLOYEE_ID" ]; then
    print_test "Delete employee (DELETE /yards/$YARD_ID/employees/$EMPLOYEE_ID)"
    DELETE_EMPLOYEE_RESPONSE=$(curl -s -w "\n%{http_code}" -X DELETE "$BASE_URL/yards/$YARD_ID/employees/$EMPLOYEE_ID")
    HTTP_CODE=$(echo "$DELETE_EMPLOYEE_RESPONSE" | tail -n 1)

    if [ "$HTTP_CODE" -eq 204 ]; then
        print_success "Employee deleted successfully"
    else
        print_error "Failed to delete employee (HTTP $HTTP_CODE)"
    fi
fi

# DELETE YARD (will cascade delete related entities)
print_test "Delete yard (DELETE /yards/$YARD_ID)"
DELETE_YARD_RESPONSE=$(curl -s -w "\n%{http_code}" -X DELETE "$BASE_URL/yards/$YARD_ID")
HTTP_CODE=$(echo "$DELETE_YARD_RESPONSE" | tail -n 1)

if [ "$HTTP_CODE" -eq 204 ]; then
    print_success "Yard deleted successfully (cascaded deletes: employees, vehicles, invites)"
else
    print_error "Failed to delete yard (HTTP $HTTP_CODE)"
fi

# VERIFY DELETION
print_test "Verify yard deletion (GET /yards/$YARD_ID should return 404)"
VERIFY_DELETE_RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/yards/$YARD_ID")
HTTP_CODE=$(echo "$VERIFY_DELETE_RESPONSE" | tail -n 1)

if [ "$HTTP_CODE" -eq 404 ]; then
    print_success "Deletion verified - yard no longer exists"
else
    print_error "Deletion verification failed (HTTP $HTTP_CODE)"
fi

#############################################
# TEST SUMMARY
#############################################

print_header "TEST SUMMARY"
echo ""
echo -e "${CYAN}Total Tests Run:     ${NC}${TESTS_RUN}"
echo -e "${GREEN}Tests Passed:        ${NC}${TESTS_PASSED}"
echo -e "${RED}Tests Failed:        ${NC}${TESTS_FAILED}"
echo ""

if [ $TESTS_FAILED -eq 0 ]; then
    echo -e "${GREEN}✓ ALL TESTS PASSED!${NC}"
    echo ""
    exit 0
else
    echo -e "${RED}✗ SOME TESTS FAILED!${NC}"
    echo ""
    exit 1
fi
