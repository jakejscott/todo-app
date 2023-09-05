import * as apprunner from "@aws-cdk/aws-apprunner-alpha";
import * as cdk from "aws-cdk-lib";
import * as apprunner_l1 from "aws-cdk-lib/aws-apprunner";
import * as ecr from "aws-cdk-lib/aws-ecr";
import * as iam from "aws-cdk-lib/aws-iam";
import * as ssm from "aws-cdk-lib/aws-ssm";
import { Construct } from "constructs";

export interface BackendStackProps extends cdk.StackProps {
  stage: string;
  service: string;
  repository: ecr.Repository;
  appVersion: string;
}

export class BackendStack extends cdk.Stack {
  public api: string;

  constructor(scope: Construct, id: string, props: BackendStackProps) {
    super(scope, id, props);

    const instanceRole = new iam.Role(this, "InstanceRole", {
      assumedBy: new iam.ServicePrincipal("tasks.apprunner.amazonaws.com"),
    });

    const accessRole = new iam.Role(this, "AccessRole", {
      assumedBy: new iam.ServicePrincipal("build.apprunner.amazonaws.com"),
    });

    props.repository.grantPull(accessRole);

    const connectionStringParam = ssm.StringParameter.fromSecureStringParameterAttributes(
      this,
      "ConnectionStringParameter",
      {
        parameterName: `/${this.stackName}/PLANET_SCALE_CONNECTION_STRING`,
      }
    );

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
          port: 80,
          environmentSecrets: {
            PLANET_SCALE_CONNECTION_STRING: apprunner.Secret.fromSsmParameter(connectionStringParam),
          },
          environmentVariables: {
            ASPNETCORE_ENVIRONMENT: "Production",
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

    this.api = `https://${service.serviceUrl}`;

    new cdk.CfnOutput(this, "ServiceUrl", {
      value: `https://${service.serviceUrl}`,
    });
  }
}
