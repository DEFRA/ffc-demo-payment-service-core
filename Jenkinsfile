@Library('defra-library@v-8') _

// buildDotNetCore environment: 'dev', project: 'FFCDemoPaymentService'

def config = [environment: 'dev', project: 'FFCDemoPaymentService']
  def tag = ''
  def mergedPrNo = ''
  def pr = ''
  def repoName = ''

  node {
    try {
      stage('Checkout source code') {
        build.checkoutSourceCode()
      }
      stage('Set PR, and tag variables') {
        (repoName, pr, tag, mergedPrNo) = build.getVariables(version.getCSProjVersion(config.project))
      }
      if (pr != '') {
        stage('Verify version incremented') {
          version.verifyCSProjIncremented(config.project)
        }
      }

      if (config.containsKey('validateClosure')) {
        config['validateClosure']()
      }

      stage('Helm lint') {
        test.lintHelm(repoName)
      }

      // if (config.containsKey('buildClosure')) {
      //   config['buildClosure']()
      // }

      // stage('Build test image') {
      //   build.buildTestImage(DOCKER_REGISTRY_CREDENTIALS_ID, DOCKER_REGISTRY, repoName, BUILD_NUMBER, tag)
      // }

      // if (fileExists('./docker-compose.snyk.yaml')){
      //   stage('Snyk test') {
      //     // ensure obj folder exists and is writable by all
      //     sh("chmod 777 ${config.project}/obj || mkdir -p -m 777 ${config.project}/obj")
      //     build.extractSynkFiles(config.project)
      //     build.snykTest(config.snykFailOnIssues, config.snykOrganisation, config.snykSeverity, "${config.project}.sln")
      //   }
      // }

      // stage('Run tests') {
      //   build.runTests(repoName, repoName, BUILD_NUMBER, tag)
      // }

      // stage('SonarCloud analysis') {
      //   test.analyseDotNetCode(repoName, BRANCH_NAME, pr)
      // }

      if (config.containsKey('testClosure')) {
        config['testClosure']()
      }

      stage('Push container image') {
        build.buildAndPushContainerImage(DOCKER_REGISTRY_CREDENTIALS_ID, DOCKER_REGISTRY, repoName, tag)
      }

      stage('Helm install') {
        helm.deployChart(config.environment, DOCKER_REGISTRY, repoName, tag)
      }

    } catch(e) {
      echo("Build failed with message: $e.message")

      stage('Send build failure slack notification') {
        notifySlack.buildFailure(e.message, '#generalbuildfailures')
      }

      if (config.containsKey('failureClosure')) {
        config['failureClosure']()
      }

      throw e
    } finally {
      if (config.containsKey('finallyClosure')) {
        config['finallyClosure']()
      }
    }
  }
