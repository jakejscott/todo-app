# todo-api

## Development

Create a `.env` file and add the following variables:

```
AWS_ACCOUNT=<AWS ACCOUNT ID>
AWS_REGION=<AWS REGION>
SERVICE=todo-api
STAGE=dev
PLANET_SCALE_CONNECTION_STRING=<PlanetScale connection string>
APP_VERSION=1.0
```

## Docker

You can run the app with docker-compose

```
docker-compose -p todo-api build --no-cache
docker-compose -p todo-api up
```

## Deployment

Create the connection string parameter in ssm parameter store

```
aws ssm put-parameter --name "/todo-api-dev-backend/PLANET_SCALE_CONNECTION_STRING" --value "<PlanetScale connection string>" --profile dev --type SecureString
```

Install CDK CLI

```
npm install -g aws-cdk
```

Change into the cdk directory and install dependencies

```
cd cdk
npm install
```

Deploy ECR Stack

```
cdk deploy todo-api-dev-ecr --profile dev
```

Push to ECR

```
aws ecr get-login-password --profile dev | docker login --username AWS --password-stdin <AWS ACCOUNT ID>.dkr.ecr.ap-southeast-2.amazonaws.com
docker-compose build
docker-compose push
```

Deploy stack

```
cdk deploy todo-api-dev-backend --profile dev
cdk deploy todo-api-dev-frontend --profile dev
```

Destroy stacks

```
cdk destroy todo-api-dev-frontend --profile dev
cdk destroy todo-api-dev-backend --profile dev
cdk destroy todo-api-dev-ecr --profile dev
```
