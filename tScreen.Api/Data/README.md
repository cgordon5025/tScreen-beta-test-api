## Setup Test UTA Data

Configure the following environment variables before running the following cURL commands:

For the local development environment use:
```bash
export TWEENSCREEN_API_URL='https://localhost:7300'
export TWEENSCREEN_API_DIR='<Directory Here>/Future Thrive/UAT'
```

For the shared development environment (azure) use:
```bash
export TWEENSCREEN_API_URL='https://tws-dev-api-app5357a5c6.azurewebsites.net'
export TWEENSCREEN_API_DIR='<Directory Here>/Future Thrive/UAT'
```

Once the cited environment variables `TWEENSCREEN_API_URL` and `TWEENSCREEN_API_DIR` are configured/set, run the commands below
Note: make sure the correct API server (`TWEENSCREEN_API_DIR`) is selected.

### Mentally Healthy Pediatrics

Person file:

```bash
curl --silent --location --request POST "$TWEENSCREEN_API_URL/api/file/bulk/person" \
--form 'companyId="A3CE84F8-60C3-4BD4-8066-6CA67E95C5AA"' \
--form 'locationId="FCBF752D-2C54-4799-B7AC-E871E660181E"' \
--form 'file=@'"$TWEENSCREEN_API_DIR"'/mentally-healthy-pediatrics/persons.csv' | jq .
```

Student: file

```bash
curl --silent --location --request POST "$TWEENSCREEN_API_URL/api/file/bulk/player" \
--form 'companyId="A3CE84F8-60C3-4BD4-8066-6CA67E95C5AA"' \
--form 'locationId="FCBF752D-2C54-4799-B7AC-E871E660181E"' \
--form 'file=@'"$TWEENSCREEN_API_DIR"'/mentally-healthy-pediatrics/students.csv' | jq .
```

### Mental Health Matters Clinic

Person file:

```bash
curl --silent --location --request POST "$TWEENSCREEN_API_URL/api/file/bulk/person" \
--form 'companyId="E2D6B08F-A64F-4187-B55C-EB9FDFBA0E58"' \
--form 'locationId="682CB710-2AFF-42A6-B52D-1DF282592099"' \
--form 'file=@'"$TWEENSCREEN_API_DIR"'/mental-health-matters-clinic/persons.csv' | jq .
```

Student: file

```bash
curl --silent --location --request POST "$TWEENSCREEN_API_URL/api/file/bulk/player" \
--form 'companyId="E2D6B08F-A64F-4187-B55C-EB9FDFBA0E58"' \
--form 'locationId="682CB710-2AFF-42A6-B52D-1DF282592099"' \
--form 'file=@'"$TWEENSCREEN_API_DIR"'/mental-health-matters-clinic/students.csv' | jq .
```

### Total Health Academy

Person file:

```bash
curl --silent --location --request POST "$TWEENSCREEN_API_URL/api/file/bulk/person" \
--form 'companyId="0A09902C-97AB-4D8F-8B82-F173EAB03528"' \
--form 'locationId="0D72CB39-B185-4E99-8688-CFDAC4CFFBDC"' \
--form 'file=@'"$TWEENSCREEN_API_DIR"'/total-health-academy/persons.csv' | jq .
```

Student: file

```bash
curl --silent --location --request POST "$TWEENSCREEN_API_URL/api/file/bulk/player" \
--form 'companyId="0A09902C-97AB-4D8F-8B82-F173EAB03528"' \
--form 'locationId="0D72CB39-B185-4E99-8688-CFDAC4CFFBDC"' \
--form 'file=@'"$TWEENSCREEN_API_DIR"'/total-health-academy/students.csv' | jq .
```

### Oct Test MFt

Person file:

```bash
curl --silent --location --request POST "$TWEENSCREEN_API_URL/api/file/bulk/person" \
--form 'companyId="650298CC-B5FE-4D28-81EB-F8150ECD9BC0"' \
--form 'locationId="BB12157A-9DA0-44BF-BE4B-85BC4DC5D746"' \
--form 'file=@'"$TWEENSCREEN_API_DIR"'/oct-test-mft/persons.csv' | jq . 
```

Student: file

```bash
curl --silent --location --request POST "$TWEENSCREEN_API_URL/api/file/bulk/player" \
--form 'companyId="650298CC-B5FE-4D28-81EB-F8150ECD9BC0"' \
--form 'locationId="BB12157A-9DA0-44BF-BE4B-85BC4DC5D746"' \
--form 'file=@'"$TWEENSCREEN_API_DIR"'/oct-test-mft/students.csv' | jq . 
```

## Big School UAT

The contents of this file were generated using the CLI tool in the `TweenScreen.TestDataGenerator` project. This 
dataset includes 100s of users to represent a school with a large population.

Person File:

```bash
curl --silent --location --request POST "$TWEENSCREEN_API_URL/api/file/bulk/person" \
--form 'companyId="3d674b04-936c-4dd5-8662-09a71365e7e6"' \
--form 'locationId="980e7669-c999-44cf-91a0-379b8cdf0a8a"' \
--form 'file=@'"$TWEENSCREEN_API_DIR"'/big-school-uat/persons.csv' | jq .
```

Student file: 

```bash
curl --silent --location --request POST "$TWEENSCREEN_API_URL/api/file/bulk/player" \
--form 'companyId="3d674b04-936c-4dd5-8662-09a71365e7e6"' \
--form 'locationId="980e7669-c999-44cf-91a0-379b8cdf0a8a"' \
--form 'file=@'"$TWEENSCREEN_API_DIR"'/big-school-uat/students.csv' | jq . 
```