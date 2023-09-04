import * as apprunner from "@aws-cdk/aws-apprunner-alpha";
import * as cdk from "aws-cdk-lib";
import * as apprunner_l1 from "aws-cdk-lib/aws-apprunner";
import * as ecr from "aws-cdk-lib/aws-ecr";
import * as iam from "aws-cdk-lib/aws-iam";
import { Construct } from "constructs";

export interface FrontendStackProps extends cdk.StackProps {
  stage: string;
  service: string;
  repository: ecr.Repository;
  appVersion: string;
}

export class FrontendStack extends cdk.Stack {
  constructor(scope: Construct, id: string, props: FrontendStackProps) {
    super(scope, id, props);

    const instanceRole = new iam.Role(this, "InstanceRole", {
      assumedBy: new iam.ServicePrincipal("tasks.apprunner.amazonaws.com"),
    });

    const accessRole = new iam.Role(this, "AccessRole", {
      assumedBy: new iam.ServicePrincipal("build.apprunner.amazonaws.com"),
    });

    props.repository.grantPull(accessRole);

    const service = new apprunner.Service(this, "Service", {
      serviceName: this.stackName,
      cpu: apprunner.Cpu.ONE_VCPU,
      memory: apprunner.Memory.TWO_GB,
      accessRole: accessRole,
      instanceRole: instanceRole,
      autoDeploymentsEnabled: false,
      vpcConnector: undefined,
      source: apprunner.Source.fromEcr({
        imageConfiguration: {
          port: 3000,
          environmentVariables: {
            NODE_ENV: "production",
          },
        },
        repository: props.repository,
        tagOrDigest: props.appVersion,
      }),
    });

    const serviceCfn = service.node.defaultChild as apprunner_l1.CfnService;

    serviceCfn.healthCheckConfiguration = {
      protocol: "HTTP",
      path: "/",
      healthyThreshold: 1,
      unhealthyThreshold: 5,
      interval: 5,
      timeout: 2,
    };

    new cdk.CfnOutput(this, "ServiceUrl", {
      value: `https://${service.serviceUrl}`,
    });
  }
}
