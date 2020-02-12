@Library('defra-library@psd-474-validate-version-increment')
import uk.gov.defra.ffc.DefraUtils
def defraUtils = new DefraUtils()

def registry = '562955126301.dkr.ecr.eu-west-2.amazonaws.com'
def regCredsId = 'ecr:eu-west-2:ecr-user'
def kubeCredsId = 'FFCLDNEKSAWSS001_KUBECONFIG'
def imageName = 'ffc-demo-payment-service-core'
def jenkinsDeployJob = 'ffc-demo-payment-service-core-deploy'
def repoName = 'ffc-demo-payment-service-core'
def projectName = 'FFCDemoPaymentService'
def pr = ''
def mergedPrNo = ''
def containerTag = ''
def sonarQubeEnv = 'SonarQube'
def sonarScanner = 'SonarScanner'
def containerSrcFolder = '\\/usr\\/src\\/app'
def localSrcFolder = '.'
def lcovFile = './test-output/lcov.info'
def timeoutInMinutes = 5


node {
  checkout scm
  try {
    stage('verify version incremented') {
      def masterVersion = defraUtils.getCSProjVersionMaster(projectName)
      def version =defraUtils.getCSProjVersion(projectName)
      if (defraUtils.versionHasIncremented(masterVersion, version)) {
        echo "version change valid '$masterVersion' -> '$version'"
      } else {
        error( "version change invalid '$masterVersion' -> '$version'")
      }
    }
    stage('Set branch, PR, and containerTag variables') {
      (pr, containerTag, mergedPrNo) = defraUtils.getVariables(repoName, defraUtils.getCSProjVersion(projectName))
      defraUtils.setGithubStatusPending()
    }
    stage('Helm lint') {
      defraUtils.lintHelm(imageName)
    }
    stage('Build test image') {
      defraUtils.buildTestImage(imageName, BUILD_NUMBER)
    }
    stage('Run tests') {
      defraUtils.runTests(imageName, BUILD_NUMBER)
    }
    stage('Push container image') {
      defraUtils.buildAndPushContainerImage(regCredsId, registry, imageName, containerTag)
    }
    if (pr == '') {
      stage('Publish chart') {
        defraUtils.publishChart(registry, imageName, containerTag)
      }
      stage('Trigger Deployment') {
        withCredentials([
          string(credentialsId: 'JenkinsDeployUrl', variable: 'jenkinsDeployUrl'),
          string(credentialsId: 'ffc-demo-payment-service-core-deploy-token', variable: 'jenkinsToken')
        ]) {
          defraUtils.triggerDeploy(jenkinsDeployUrl, jenkinsDeployJob, jenkinsToken, ['chartVersion':'1.0.0'])
        }
      }
    } else {
      stage('Helm install') {
        withCredentials([
          string(credentialsId: 'sqsQueueEndpoint', variable: 'sqsQueueEndpoint'),
          string(credentialsId: 'scheduleQueueUrlPR', variable: 'scheduleQueueUrl'),
          string(credentialsId: 'scheduleQueueAccessKeyIdListen', variable: 'scheduleQueueAccessKeyId'),
          string(credentialsId: 'scheduleQueueSecretAccessKeyListen', variable: 'scheduleQueueSecretAccessKey'),
          string(credentialsId: 'paymentQueueUrlPR', variable: 'paymentQueueUrl'),
          string(credentialsId: 'paymentQueueAccessKeyIdListen', variable: 'paymentQueueAccessKeyId'),
          string(credentialsId: 'paymentQueueSecretAccessKeyListen', variable: 'paymentQueueSecretAccessKey'),
          string(credentialsId: 'postgresExternalNamePaymentsCore', variable: 'postgresExternalName'),
          string(credentialsId: 'postgresConnectionStringPaymentsCore', variable: 'postgresConnectionString')
        ]) {
          def helmValues = [
            /container.scheduleQueueEndpoint="$sqsQueueEndpoint"/,
            /container.scheduleQueueUrl="$scheduleQueueUrl"/,
            /container.scheduleQueueAccessKeyId="$scheduleQueueAccessKeyId"/,
            /container.scheduleQueueSecretAccessKey="$scheduleQueueSecretAccessKey"/,
            /container.scheduleCreateQueue="false"/,
            /container.paymentQueueEndpoint="$sqsQueueEndPoint"/,
            /container.paymentQueueUrl="$paymentQueueUrl"/,
            /container.paymentQueueAccessKeyId="$paymentQueueAccessKeyId"/,
            /container.paymentQueueSecretAccessKey="$paymentQueueSecretAccessKey"/,
            /container.paymentCreateQueue="false"/,
            /container.redeployOnChange="$pr-$BUILD_NUMBER"/,
            /postgresExternalName="$postgresExternalName"/,
            /postgresConnectionString="$postgresConnectionString"/
          ].join(',')

          def extraCommands = [
            "--values ./helm/ffc-demo-payment-service-core/jenkins-aws.yaml",
            "--set $helmValues"
          ].join(' ')

          defraUtils.deployChart(kubeCredsId, registry, imageName, containerTag, extraCommands)
          echo "Build available for review"
        }
      }
    }
    if (mergedPrNo != '') {
      stage('Remove merged PR') {
        defraUtils.undeployChart(kubeCredsId, imageName, mergedPrNo)
      }
    }
    defraUtils.setGithubStatusSuccess()
  } catch(e) {
    defraUtils.setGithubStatusFailure(e.message)
    throw e
  } 
}
