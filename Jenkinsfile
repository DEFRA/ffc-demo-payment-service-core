@Library('defra-library@0.0.8')
import uk.gov.defra.ffc.DefraUtils
def defraUtils = new DefraUtils()

def registry = '562955126301.dkr.ecr.eu-west-2.amazonaws.com'
def regCredsId = 'ecr:eu-west-2:ecr-user'
def kubeCredsId = 'FFCLDNEKSAWSS001_KUBECONFIG'
def imageName = 'ffc-demo-payment-service-core'
def jenkinsDeployJob = 'ffc-demo-payment-service-core-deploy'
def repoName = 'ffc-demo-payment-service-core'
def pr = ''
def mergedPrNo = ''
def containerTag = ''
def sonarQubeEnv = 'SonarQube'
def sonarScanner = 'SonarScanner'
def containerSrcFolder = '\\/usr\\/src\\/app'
def localSrcFolder = '.'
def lcovFile = './test-output/lcov.info'
def timeoutInMinutes = 5

def runTests (name, suffix) {
  try {
    sh 'mkdir -p test-output'
    sh 'chmod 777 test-output'
    sh "docker-compose -p $name-$suffix-$containerTag -f docker-compose.test.yaml run $name-test"​
  } finally {
    sh "docker-compose -p $name-$suffix-$containerTag -f docker-compose.test.yaml down -v"
  }
}

def buildTestImage(name, suffix) {
  sh 'docker image prune -f || echo could not prune images'
  sh "docker-compose -p $name-$suffix-$containerTag -f docker-compose.test.yaml build --no-cache $name-test"
}

node {
  checkout scm
  try {
    stage('Set branch, PR, and containerTag variables') {
      (pr, containerTag, mergedPrNo) = defraUtils.getVariables(repoName)
      defraUtils.setGithubStatusPending()
    }
    stage('Helm lint') {
      defraUtils.lintHelm(imageName)
    }
    stage('Build test image') {
      buildTestImage(imageName, BUILD_NUMBER)
    }
    stage('Run tests') {
      runTests(imageName, BUILD_NUMBER)
    }
    stage('Push container image') {
      defraUtils.buildAndPushContainerImage(regCredsId, registry, imageName, containerTag)
    }
    if (pr == '') {
      stage('Publish chart') {
        defraUtils.publishChart(registry, imageName, containerTag)
      }
      // stage('Trigger Deployment') {
      //   withCredentials([
      //     string(credentialsId: 'JenkinsDeployUrl', variable: 'jenkinsDeployUrl'),
      //     string(credentialsId: 'ffc-demo-payment-service-core-deploy-token', variable: 'jenkinsToken')
      //   ]) {
      //     defraUtils.triggerDeploy(jenkinsDeployUrl, jenkinsDeployJob, jenkinsToken, ['chartVersion':'1.0.0'])
      //   }
      //}
    } else {
      stage('Helm install') {
        withCredentials([
          string(credentialsId: 'messageQueueHostPR', variable: 'messageQueueHost'),
          usernamePassword(credentialsId: 'scheduleListenPR', usernameVariable: 'scheduleQueueUsername', passwordVariable: 'scheduleQueuePassword'),
          usernamePassword(credentialsId: 'paymentListenPR', usernameVariable: 'paymentQueueUsername', passwordVariable: 'paymentQueuePassword'),
          string(credentialsId: 'postgresExternalNamePaymentsPR', variable: 'postgresExternalName'),
          usernamePassword(credentialsId: 'postgresPaymentsPR', usernameVariable: 'postgresUsername', passwordVariable: 'postgresPassword'),
        ]) {
          def helmValues = [
            /container.messageQueueHost="$messageQueueHost"/,
            /container.paymentQueuePassword="$paymentQueuePassword"/,
            /container.paymentQueueUser="$paymentQueueUsername"/,
            /container.redeployOnChange="$pr-$BUILD_NUMBER"/,
            /container.scheduleQueuePassword="$scheduleQueuePassword"/,
            /container.scheduleQueueUser="$scheduleQueueUsername"/,
            /postgresExternalName="$postgresExternalName"/,
            /postgresPassword="$postgresPassword"/,
            /postgresUsername="$postgresUsername"/
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
