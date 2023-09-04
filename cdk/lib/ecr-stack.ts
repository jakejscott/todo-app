import * as cdk from "aws-cdk-lib";
import * as ecr from "aws-cdk-lib/aws-ecr";
import { Construct } from "constructs";

export interface EcrStackProps extends cdk.StackProps {
  stage: string;
  service: string;
}

export class EcrStack extends cdk.Stack {
  public backendRepository: ecr.Repository;
  public frontendRepository: ecr.Repository;

  constructor(scope: Construct, id: string, props: EcrStackProps) {
    super(scope, id, props);

    this.backendRepository = new ecr.Repository(this, "BackendRepository", {
      repositoryName: `${this.stackName}-backend`,
      encryption: ecr.RepositoryEncryption.AES_256,
      imageScanOnPush: true,
      lifecycleRules: [
        {
          rulePriority: 1,
          description: "Remove if more than 3 images",
          tagStatus: ecr.TagStatus.ANY,
          maxImageCount: 3,
        },
      ],
      removalPolicy: cdk.RemovalPolicy.DESTROY,
      autoDeleteImages: true,
      imageTagMutability: ecr.TagMutability.MUTABLE,
    });

    this.frontendRepository = new ecr.Repository(this, "FrontendRepository", {
      repositoryName: `${this.stackName}-frontend`,
      encryption: ecr.RepositoryEncryption.AES_256,
      imageScanOnPush: true,
      lifecycleRules: [
        {
          rulePriority: 1,
          description: "Remove if more than 3 images",
          tagStatus: ecr.TagStatus.ANY,
          maxImageCount: 3,
        },
      ],
      removalPolicy: cdk.RemovalPolicy.DESTROY,
      autoDeleteImages: true,
      imageTagMutability: ecr.TagMutability.MUTABLE,
    });
  }
}
