@Library('defra-library@2.0.0')
import uk.gov.defra.ffc.DefraUtils
def defraUtils = new DefraUtils()

def containerSrcFolder = '\\/usr\\/src\\/app'
def csProjectName = 'FFCDemoPaymentService'
def containerTag = ''
def lcovFile = './test-output/lcov.info'
def localSrcFolder = '.'
def mergedPrNo = ''
def pr = ''
def serviceName = 'ffc-demo-payment-service-core'
def sonarQubeEnv = 'SonarQube'
def sonarScanner = 'SonarScanner'
def timeoutInMinutes = 5

node {
  checkout scm
  try {
    stage('Set GitHub status as pending'){
      defraUtils.setGithubStatusPending()
    }
    stage('Set branch, PR, and containerTag variables') {
      (pr, containerTag, mergedPrNo) = defraUtils.getVariables(serviceName, defraUtils.getCSProjVersion(csProjectName)) 
    }
    stage('Helm lint') {
      defraUtils.lintHelm(serviceName)
    }
    stage('Build test image') {
      defraUtils.buildTestImage(DOCKER_REGISTRY_CREDENTIALS_ID, DOCKER_REGISTRY, serviceName, BUILD_NUMBER)
    }
    stage('Run tests') {
      defraUtils.runTests(serviceName, serviceName, BUILD_NUMBER)
    }
    stage('Push container image') {
      defraUtils.buildAndPushContainerImage(DOCKER_REGISTRY_CREDENTIALS_ID, DOCKER_REGISTRY, serviceName, containerTag)
    } 

    if (pr == '') {
      stage('Publish chart') {
        defraUtils.publishChart(DOCKER_REGISTRY, serviceName, containerTag)
      }
      stage('Trigger Release') {
        withCredentials([
          string(credentialsId: 'github-auth-token', variable: 'gitToken') 
        ]) {
          defraUtils.triggerRelease(containerTag, serviceName, containerTag, gitToken)
        }
      }
      stage('Trigger Deployment') {
        withCredentials([
          string(credentialsId: 'payment-service-core-deploy-token', variable: 'jenkinsToken'),
          string(credentialsId: 'payment-service-core-job-deploy-name', variable: 'deployJobName')
        ]) {
          defraUtils.triggerDeploy(JENKINS_DEPLOY_SITE_ROOT, deployJobName, jenkinsToken, ['chartVersion':containerTag])
        }
      }
    } else {
      stage('Verify version incremented') {
        defraUtils.verifyCSProjVersionIncremented(csProjectName)
      }
      stage('Helm install') {
        withCredentials([
          string(credentialsId: 'sqs-queue-endpoint', variable: 'sqsQueueEndpoint'),
          string(credentialsId: 'schedule-queue-url-pr', variable: 'scheduleQueueUrl'),
          string(credentialsId: 'schedule-queue-access-key-id-listen', variable: 'scheduleQueueAccessKeyId'),
          string(credentialsId: 'schedule-queue-secret-access-key-listen', variable: 'scheduleQueueSecretAccessKey'),
          string(credentialsId: 'payment-queue-url-pr', variable: 'paymentQueueUrl'),
          string(credentialsId: 'payment-queue-access-key-id-listen', variable: 'paymentQueueAccessKeyId'),
          string(credentialsId: 'payment-queue-secret-access-key-listen', variable: 'paymentQueueSecretAccessKey'),
          string(credentialsId: 'postgres-external-name-pr', variable: 'postgresExternalName'),
          string(credentialsId: 'payments-service-core-postgres-connection-string', variable: 'postgresConnectionString')
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

          defraUtils.deployChart(KUBE_CREDENTIALS_ID, DOCKER_REGISTRY, serviceName, containerTag, extraCommands)
          echo "Build available for review"
        }
      }
    }
    if (mergedPrNo != '') {
      stage('Remove merged PR') {
        defraUtils.undeployChart(KUBE_CREDENTIALS_ID, serviceName, mergedPrNo)
      }
    }
    stage('Set GitHub status as success'){
      defraUtils.setGithubStatusSuccess()
    }
  } catch(e) {
    defraUtils.setGithubStatusFailure(e.message)
    defraUtils.notifySlackBuildFailure(e.message, "#generalbuildfailures")
    throw e
  } 
}
