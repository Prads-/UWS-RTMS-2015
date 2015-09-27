onStart();
var intervention = { "Intervention_Name": "", "Intervention_Description": "", "Investigators": [], "Tests": [] };

var Test = function () {
    this.Test_Id = 0;
    this.Test_Name = "";
    this.Test_Description = "";
    this.Questions = [];
};

var Question = function () {
    this.Question_Id = 0;
    this.Question_Title = "";
    this.Answer_Type = "";
    this.Answers = [];
};
var currentTestId = 0;
var currentQuestionId = 0;
var editingTest = false;
var editingQuestion = false;
var editNotSelected = false;
var externalTest = 0;
var tempTest = new Test();
var tempQuestion = new Question();

function createTest()
{
    var test = new Test(); //create test object
    test.Test_Id = intervention.Tests.length + 1; //Id is +1 of length, starting id = 1
    currentTestId = test.Test_Id; //the current test's id
    intervention.Tests.push(test); //push new test object to intervention object
    printMessage("New test has been created", "success");
}

function createExternalTest()
{
    if (externalTest == 0) {
        var test = new Test(); //create test object
        test.Test_Id = intervention.Tests.length + 1; //Id is +1 of length, starting id = 1
        test.Test_Name = "Mockup External Test";
        test.Test_Description = "This is a mockup external test that will be used to demonstrate the use of an external API";
        currentTestId = test.Test_Id; //the current test's id
        intervention.Tests.push(test); //push new test object to intervention object
        printMessage("New external test has been created", "success");
    } else {
        currentTestId = externalTest;
        printMessage("You are now editing the external test", "edit");
    }
}

function editTest()
{
    var testSelect = document.getElementById("testSelect");
    if (testSelect.selectedIndex < 0) {
        printMessage("No test has been selected for editing", "fail");
        editNotSelected = true;
    } else {
        editingTest = true;
        currentTestId = testSelect.options[testSelect.selectedIndex].value; //set currentTestId to test selected from list
        testIndex = getTestIndex(currentTestId);
        tempTest = clone(intervention.Tests[testIndex]);
        document.getElementById("testName").value = intervention.Tests[testIndex].Test_Name;
        document.getElementById("testDescription").value = intervention.Tests[testIndex].Test_Description;
        var questionSelect = document.getElementById("questionSelect");
        for (var i = 0; i < intervention.Tests[testIndex].Questions.length; i++) {
            var option = document.createElement("option");
            option.text = intervention.Tests[testIndex].Questions[i].Question_Title;
            option.value = intervention.Tests[testIndex].Questions[i].Question_Id;
            questionSelect.add(option);
        }
        printMessage("You are now editing an existing test", "edit");
    }
}

function deleteTest()
{
    var testSelect = document.getElementById("testSelect");
    if (testSelect.selectedIndex < 0) {
        printMessage("No test has been selected for deleting", "fail");
    } else {
        testId = testSelect.options[testSelect.selectedIndex].value; //testId to be deleted
        var testIndex = getTestIndex(testId);
        intervention.Tests.splice(testIndex, 1);
        testSelect.remove(testSelect.selectedIndex);
        printMessage("Test has been deleted", "fail");
    }
}

function createQuestion()
{
    var question = new Question(); //create question object
    var testIndex = getTestIndex(currentTestId);
    question.Question_Id = intervention.Tests[testIndex].Questions.length + 1; //Id is +1 of length, starting id = 1
    currentQuestionId = question.Question_Id; //the current question's id
    intervention.Tests[testIndex].Questions.push(question);
    printMessage("New question has been created", "success");
}

function editQuestion()
{
    if (questionSelect.selectedIndex < 0) {
        printMessage("No question has been selected for editing", "fail");
        editNotSelected = true;
    } else {
        editingQuestion = true;
        currentQuestionId = questionSelect.options[questionSelect.selectedIndex].value;
        testIndex = getTestIndex(currentTestId);
        questionIndex = getQuestionIndex(testIndex, currentQuestionId);
        tempQuestion = clone(intervention.Tests[testIndex].Questions[questionIndex])
        document.getElementById("questionTitle").value = intervention.Tests[testIndex].Questions[questionIndex].Question_Title; //prefill questionTitle
        document.getElementById("answerType").value = intervention.Tests[testIndex].Questions[questionIndex].Answer_Type; //prefill answerType
        questionTypeChange(intervention.Tests[testIndex].Questions[questionIndex].Answer_Type);
        var answersSelect = document.getElementById("answersSelect");
        answers = intervention.Tests[testIndex].Questions[questionIndex].Answers.slice();
        for (var i = 0; i < intervention.Tests[testIndex].Questions[questionIndex].Answers.length; i++) { //prefill answers list
            var option = document.createElement("option");
            option.text = intervention.Tests[testIndex].Questions[questionIndex].Answers[i];
            answersSelect.add(option);
        }
        printMessage("You are now editing an existing question", "edit");
    }
}

function deleteQuestion() {
    var questionSelect = document.getElementById("questionSelect");
    if (questionSelect.selectedIndex < 0) {
        printMessage("No question has been selected for deleting", "fail");
    } else {
        questionId = questionSelect.options[questionSelect.selectedIndex].value; //questionId to be deleted
        var testIndex = getTestIndex(currentTestId);
        var questionIndex = getQuestionIndex(testIndex, questionId);
        intervention.Tests[testIndex].Questions.splice(questionIndex, 1);
        questionSelect.remove(questionSelect.selectedIndex);
        printMessage("Question has been deleted", "fail");
    }
}

function saveTest()
{
    var testName = document.getElementById("testName").value;
    var select = document.getElementById("testSelect");
    var options = select.options;
    var testIndex = getTestIndex(currentTestId);
    var optionIndex = checkSelect(options, currentTestId);
    if (optionIndex != -1) { //select already has test id
        select.options[optionIndex].text = testName; //set test name again in case name was edited 
    } else {                 //select doesn't have test id
        var option = document.createElement("option");
        option.value = currentTestId
        option.text = testName;
        select.add(option); //add new
    }
    intervention.Tests[testIndex].Test_Name = testName;
    intervention.Tests[testIndex].Test_Description = document.getElementById("testDescription").value;
    currentTestId = 0;
    printMessage("Test has been saved", "success");
    resetTestView();
}

function saveExternalTest()
{
    var questions = [];
    if (document.getElementById("externalQuestion1").checked) {
        var question = new Question();
        question.Question_Id = questions.length + 1;
        question.Question_Title = document.getElementById("externalQuestion1").value;
        question.Answer_Type = 5; //external test answer type
        questions.push(question);
    }
    if (document.getElementById("externalQuestion2").checked) {
        var question = new Question();
        question.Question_Id = questions.length + 1;
        question.Question_Title = document.getElementById("externalQuestion2").value;
        question.Answer_Type = 5; //external test answer type
        questions.push(question);
    }
    if (document.getElementById("externalScore1").checked) {
        var question = new Question();
        question.Question_Id = questions.length + 1;
        question.Question_Title = document.getElementById("externalScore1").value;
        question.Answer_Type = 5; //external test answer type
        questions.push(question);
    }
    if (document.getElementById("externalScore2").checked) {
        var question = new Question();
        question.Question_Id = questions.length + 1;
        question.Question_Title = document.getElementById("externalScore2").value;
        question.Answer_Type = 5; //external test answer type
        questions.push(question);
    }
    if (externalTest > 0) { //already an existing external test
        testIndex = getTestIndex(externalTest);
        intervention.Tests[testIndex].Questions = questions.slice();
        printMessage("External test has been edited", "success");
    } else {                //save new external test
        externalTest = currentTestId;
        testIndex = getTestIndex(currentTestId);
        intervention.Tests[testIndex].Questions = questions.slice();
        var testSelect = document.getElementById("testSelect");
        var option = document.createElement("option");
        option.value = currentTestId
        option.text = "Mockup External Test";
        testSelect.add(option); //add new
        printMessage("New external test has been saved", "success");
    }
}

function saveQuestion()
{
    var questionTitle = document.getElementById("questionTitle").value;
    var select = document.getElementById("questionSelect");
    var answerType = document.getElementById("answerType");
    var testIndex = getTestIndex(currentTestId);
    var questionIndex = getQuestionIndex(testIndex, currentQuestionId);
    var options = select.options;
    var optionIndex = checkSelect(options, currentQuestionId);
    if (optionIndex != -1) { //select already has question id
        select.options[optionIndex].text = questionTitle;
    } else {                 //select doesn't have question id
        var option = document.createElement("option");
        option.value = currentQuestionId;
        option.text = questionTitle;
        select.add(option); //add new
    }
    intervention.Tests[testIndex].Questions[questionIndex].Question_Title = questionTitle
    intervention.Tests[testIndex].Questions[questionIndex].Answer_Type = answerType.options[answerType.selectedIndex].value;
    currentQuestionId = 0;
    printMessage("Question has been saved", "success");
    resetQuestionView();
}

function createIntervention() {
    intervention.Intervention_Name = document.getElementById("interventionName").value;
    intervention.Intervention_Description = document.getElementById("interventionDescription").value;
    //printMessage("Intervention was submitted");
    console.log(intervention);
}

function listInvestigators(rows) {
    var select = document.getElementById("investigatorSelect");
    for (var i = 0; i < rows.length; i++) {
        var option = document.createElement("option");
        option.text = i + 1 + ") " + rows[i].First_Name + " " + rows[i].Last_Name + " " + rows[i].Institution;
        option.value = rows[i].Id.toString();
        select.appendChild(option);
    }
}

function addInvestigators() {
    var select = document.getElementById("investigatorSelect");
    var selectedInvestigators = [];
    var investigatorList = "";
    for (var i = 0; i < select.length; i++) {
        if (select.options[i].selected) {
            selectedInvestigators.push(select.options[i].value);
            investigatorList += select.options[i].text + ", ";
        }
    }
    intervention.Investigators = selectedInvestigators;
    document.getElementById("selectedInvestigators").textContent = investigatorList.slice(0, -2);
    printMessage("Investigators successfully added", "success");
}

function addAnswer() {
    if (document.getElementById("answer").value != "") { //answer cannot be blank
        var select = document.getElementById("answersSelect");
        var answer = document.getElementById("answer").value;
        document.getElementById("answer").value = "";
        var option = document.createElement("option");
        option.text = answer;
        select.add(option);
        var testIndex = getTestIndex(currentTestId);
        var questionIndex = getQuestionIndex(testIndex, currentQuestionId);
        intervention.Tests[testIndex].Questions[questionIndex].Answers.push(answer);
    }
}

function deleteAnswer() {
    var answersSelect = document.getElementById("answersSelect");
    if (answersSelect.selectedIndex < 0) {
        printMessage("No answer has been selected for deleting", "fail");
    } else {
        answer = answersSelect.options[answersSelect.selectedIndex].value; //questionId to be deleted
        var testIndex = getTestIndex(currentTestId);
        var questionIndex = getQuestionIndex(testIndex, currentQuestionId);
        var answerIndex = intervention.Tests[testIndex].Questions[questionIndex].Answers.indexOf(answer);
        intervention.Tests[testIndex].Questions[questionIndex].Answers.splice(answerIndex, 1);
        answersSelect.remove(answersSelect.selectedIndex);
        printMessage("Answer has been deleted", "fail");
    }
}

function questionTypeChange(selectedOption) {
    if (selectedOption == null) {
        var select = document.getElementById("answerType");
        selectedOption = select.options[select.selectedIndex].value;
    }
    var testIndex = getTestIndex(currentTestId);
    var questionIndex = getQuestionIndex(testIndex, currentQuestionId);
    if (selectedOption == "3") {
        document.getElementById("answer").style.display = "none";
        document.getElementById("addAnswerBtn").style.display = "hidden";
        document.getElementById("measurementType").style.display = "block";
        emptySelect(document.getElementById("answersSelect"));
        intervention.Tests[testIndex].Questions[questionIndex].Answers = [];
    } else if (selectedOption == "4") {
        document.getElementById("answer").style.display = "none";
        document.getElementById("addAnswerBtn").style.display = "none";
        document.getElementById("measurementType").style.display = "none";
        emptySelect(document.getElementById("answersSelect"));
        intervention.Tests[testIndex].Questions[questionIndex].Answers = [];
    } else {
        document.getElementById("answer").style.display = "block";
        document.getElementById("addAnswerBtn").style.display = "block";
        document.getElementById("measurementType").style.display = "none";
    }
}

function backTest()
{
    if(editingTest)
    {
        resetTestView();
        var testIndex = getTestIndex(currentTestId);
        intervention.Tests[testIndex] = clone(tempTest);
        currentTestId = 0;
        printMessage("Test was not edited", "fail");
        editingTest = false;
    } else {
        discardTest();
    }
}

function discardTest() {
    resetTestView();
    var testIndex = getTestIndex(currentTestId);
    intervention.Tests.splice(testIndex, 1);
    currentTestId = 0;
    printMessage("Test has been discarded", "fail");
}

function discaredExternalTest() {
    if (externalTest == 0) {
        var testIndex = getTestIndex(currentTestId);
        intervention.Tests.splice(testIndex, 1);
        printMessage("External test has been discarded", "fail");
    } else {
        printMessage("External test was not edited", "fail")
    }
}

function backQuestion()
{
    if(editingQuestion)
    {
        resetQuestionView();
        var testIndex = getTestIndex(currentTestId);
        var questionIndex = getQuestionIndex(testIndex, currentQuestionId);
        intervention.Tests[testIndex].Questions[questionIndex] = clone(tempQuestion);
        currentQuestionId = 0;
        printMessage("Question was not edited", "fail");
        editingQuestion = false;
    } else {
        discardQuestion();
    }
}

function discardQuestion()
{
    resetQuestionView();
    var testIndex = getTestIndex(currentTestId);
    var questionIndex = getQuestionIndex(testIndex, currentQuestionId);
    intervention.Tests[testIndex].Questions.splice(questionIndex, 1);
    currentQuestionId = 0;
    printMessage("Question has been discarded", "fail");
}

function resetTestView() {
    document.getElementById("testName").value = "";
    document.getElementById("testDescription").value = "";
    emptySelect(document.getElementById("questionSelect"));
}

function resetQuestionView()
{
    document.getElementById("questionTitle").value = "";
    document.getElementById("answerType").selectedIndex = 0;
    emptySelect(document.getElementById("answersSelect"));
    document.getElementById("measurementType").selectedIndex = 0;
    document.getElementById("answer").value = "";
    document.getElementById("answer").style.display = "block";
    document.getElementById("addAnswerBtn").style.display = "block";
    document.getElementById("measurementType").style.display = "none";
}

function getTestIndex(testId) {
    for (var i = 0; i < intervention.Tests.length; i++) {
        if (intervention.Tests[i].Test_Id == testId) {
            return i;
        }
    }
}

function getQuestionIndex(testIndex, questionId) {
    for (var i = 0; i < intervention.Tests[testIndex].Questions.length; i++) {
        if (intervention.Tests[testIndex].Questions[i].Question_Id == questionId) {
            return i;
        }
    }
}

function checkSelect(options, id)
{
    for (var i = 0; i < options.length; i++) {
        if (options[i].value == id) {
            return i;
        }
    }
    return -1;
}

function checkTestSelect()
{
    var testSelect = document.getElementById("testSelect");
    if (testSelect.options[testSelect.selectedIndex].text == "Mockup External Test") {
        document.getElementById("editTestBtn").disabled = true;
    } else {
        document.getElementById("editTestBtn").disabled = false;
    }
}

function emptySelect(select) {
    while (select.options.length > 0)
        select.remove(0);
}

function printMessage(message, status)
{
    var messageElement = document.getElementById('message');
    switch(status){
        case "success":
            messageElement.className = "success";
            break;
        case "fail":
            messageElement.className = "fail";
            break;
        case "edit":
            messageElement.className = "edit";
        default:
            break;
    }
    messageElement.style.display = "inline";
    messageElement.innerHTML = message;
    setTimeout(function () {
        messageElement.innerHTML = "";
        messageElement.style.display = "none";
    }, 3000);
}

function clone(obj)
{
    var newObj = JSON.parse(JSON.stringify(obj))
    return newObj;
}

function onStart()
{
    document.getElementById('Investigators').style.display = "block";
    document.getElementById('Tests').style.display = "block";
    document.getElementById('ExternalTest').style.display = "block";
    document.getElementById('AddQuestion').style.display = "block";
    document.getElementById("message").style.display = "block";
    document.getElementById('measurementType').style.display = "none";
    printMessage("Intervention setup has started!", "success");
}

//--------------------------------------------------AngularJS--------------------------------------------------
var InterventionSetupApp = angular.module('InterventionSetupApp', []);

InterventionSetupApp.controller('InterventionSetupController', function ($scope, $http) {
    $scope.index = 1;
    $scope.interventionObj = intervention;
    getInvestigators();
    $scope.check = function (i) {
        if ($scope.index == i)
            return true;
        else
            return false;
    };

    $scope.setIndex = function (i) {
        if(!editNotSelected) //only change index if editNotSelected is false
            $scope.index = i;
        else
            editNotSelected = false;
    };

    $scope.submitIntervention = function () {
        createIntervention();
        $http.post('/Administrator/SubmitIntervention', JSON.stringify(intervention)).
            then(function (response) {
                printMessage("Response:" + response.data, "success");
            }, function (error) {
        });
    };

    function getInvestigators() {
        $http.get('/Administrator/GetInvestigators').
          then(function (results) {
              var obj = results.data;
              $scope.result = obj;
              listInvestigators(obj);
          }, function (error) {
        });
    }
});
//-------------------------------------------------------------------------------------------------------------

