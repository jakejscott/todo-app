#!/usr/bin/env node
import "source-map-support/register";
import * as cdk from "aws-cdk-lib";
import * as dotenv from "dotenv";
import { get } from "env-var";
import { EcrStack } from "../lib/ecr-stack";
import { BackendStack } from "../lib/backend-stack";

dotenv.config({
  path: "../.env",
});

const AWS_ACCOUNT = get("AWS_ACCOUNT").required().asString();
const AWS_REGION = get("AWS_REGION").required().asString();
const SERVICE = get("SERVICE").required().asString();
const STAGE = get("STAGE").required().asString();
const VERSION = get("VERSION").required().asString();

const app = new cdk.App();

const ecrStack = new EcrStack(app, `${SERVICE}-${STAGE}-ecr`, {
  description: `${SERVICE} ${STAGE} ecr`,
  service: SERVICE,
  stage: STAGE,
  env: {
    account: AWS_ACCOUNT,
    region: AWS_REGION,
  },
});

new BackendStack(app, `${SERVICE}-${STAGE}-backend`, {
  description: `${SERVICE} ${STAGE} backend`,
  service: SERVICE,
  stage: STAGE,
  env: {
    account: AWS_ACCOUNT,
    region: AWS_REGION,
  },
  repository: ecrStack.backendRepository,
  version: VERSION,
});
