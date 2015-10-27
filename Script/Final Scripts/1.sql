CREATE TABLE Person (
	Id INT IDENTITY(1,1) NOT NULL,
	First_Name VARCHAR(255),
	Last_Name VARCHAR(255),
	Phone_Number VARCHAR(15),
	Street VARCHAR(255),
	Suburb VARCHAR(255),
	State VARCHAR(255),
	Postcode VARCHAR(4),
	Email VARCHAR(255),
	PRIMARY KEY (Id));

CREATE TABLE Trial (
	Id INT IDENTITY(1,1) NOT NULL, 
	Name VARCHAR(255), 
	Description VARCHAR(MAX), 
	Start_Date DATE, 
	End_Date DATE,
	Objectives VARCHAR(MAX), 
	Hypothesis VARCHAR(MAX), 
	Outcome VARCHAR(MAX),
	HasBeenRandomised BIT, 
	TermsAndConditions VARCHAR(MAX),
	PRIMARY KEY (Id));

CREATE TABLE Clinician (
	Id INT IDENTITY(1,1) NOT NULL, 
	Person_Id INT,
	PRIMARY KEY (Id),
	FOREIGN KEY (Person_Id) REFERENCES Person(Id));

CREATE TABLE Participant (
	Id INT IDENTITY(1,1) NOT NULL, 
	Person_Id INT,
	Date_Of_Birth Date,
	Gender VARCHAR(10),
	PRIMARY KEY (Id),
	FOREIGN KEY (Person_Id) REFERENCES Person(Id));

CREATE TABLE Administrator (
	Id INT IDENTITY(1,1) NOT NULL, 
	Person_Id INT,
	PRIMARY KEY (Id),
	FOREIGN KEY (Person_Id) REFERENCES Person(Id));

CREATE TABLE Participant_Clinician(
	Id INT IDENTITY(1,1) NOT NULL,
	Participant_Id INT,
	Clinician_Id INT,
	PRIMARY KEY (Id),
	FOREIGN KEY (Participant_Id) REFERENCES Participant(Id),
	FOREIGN KEY (Clinician_Id) REFERENCES Clinician(Id));

CREATE TABLE Trial_Participant (
	Id INT IDENTITY(1,1) NOT NULL, 
	Trial_Id INT, 
	Participant_Id INT, 
	Classification INT,
	PRIMARY KEY (Id), 
	FOREIGN KEY (Trial_Id) REFERENCES Trial(Id), 
	FOREIGN KEY (Participant_Id) REFERENCES Participant(Id));

CREATE TABLE Participant_Group (
	Id INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(255),
	PRIMARY KEY (Id));

CREATE TABLE Trial_Participant_Participant_Group (
	Id INT IDENTITY(1,1) NOT NULL,
	Trial_Participant_Id INT,
	Participant_Group_Id INT,
	PRIMARY KEY (Id),
	FOREIGN KEY (Trial_Participant_Id) REFERENCES Trial_Participant(Id),
	FOREIGN KEY (Participant_Group_Id) REFERENCES Participant_Group(Id));

CREATE TABLE Investigator (
	Id INT IDENTITY(1,1) NOT NULL, 
	Person_Id INT,
	Institution VARCHAR(255), 
	PRIMARY KEY (Id),
	FOREIGN KEY (Person_Id) REFERENCES Person(Id));

CREATE TABLE Trial_Investigator (
	Id INT IDENTITY(1,1) NOT NULL, 
	Investigator_Id INT, 
	Trial_Id INT, 
	Type INT,
	PRIMARY KEY (Id),
	FOREIGN KEY (Investigator_Id) REFERENCES Investigator(Id), 
	FOREIGN KEY (Trial_Id) REFERENCES Trial(Id));

CREATE TABLE Screening_Criteria (
	Id INT IDENTITY(1,1) NOT NULL,
	Description VARCHAR(1000),
	PRIMARY KEY (Id));

CREATE TABLE Screening_Criteria_Option (
	Id INT IDENTITY(1,1) NOT NULL,
	Screening_Criteria_Id INT,
	Description VARCHAR(1000),
	PRIMARY KEY (Id),
	FOREIGN KEY (Screening_Criteria_Id) REFERENCES Screening_Criteria(Id));

CREATE TABLE Trial_Screening_Criteria (
	Id INT IDENTITY(1,1) NOT NULL,
	Trial_Id INT,
	Screening_Criteria_Id INT,
	OperatorType INT,
	OperatorValue VARCHAR(MAX),
	PRIMARY KEY (Id),
	FOREIGN KEY (Trial_Id) REFERENCES Trial(Id),
	FOREIGN KEY (Screening_Criteria_Id) REFERENCES Screening_Criteria(Id));

CREATE TABLE Participant_Screening_Criteria (
	Id INT IDENTITY(1,1) NOT NULL,
	Participant_Id INT,
	Screening_Criteria_Id INT,
	Value VARCHAR(MAX),
	PRIMARY KEY (Id),
	FOREIGN KEY (Screening_Criteria_Id) REFERENCES Screening_Criteria(Id),
	FOREIGN KEY (Participant_Id) REFERENCES Participant(Id));

CREATE TABLE Intervention_Area (
	Id INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(1000),
	PRIMARY KEY (Id));

CREATE TABLE Intervention_Area_Test (
	Id INT IDENTITY(1,1) NOT NULL,
	Intervention_Area_Id INT,
	Name VARCHAR(1000),
	PRIMARY KEY (Id),
	FOREIGN KEY (Intervention_Area_Id) REFERENCES Intervention_Area(Id));

CREATE TABLE Intervention_Area_Test_Question (
	Id INT IDENTITY(1,1) NOT NULL,
	Intervention_Area_Test_Id INT,
	Question VARCHAR(MAX),
	Question_Type INT,
	PRIMARY KEY (Id),
	FOREIGN KEY (Intervention_Area_Test_Id) REFERENCES Intervention_Area_Test(Id));

CREATE TABLE Intervention_Area_Test_Question_Option (
	Id INT IDENTITY(1,1) NOT NULL,
	Intervention_Area_Test_Question_Id INT,
	Opt VARCHAR(MAX),
	PRIMARY KEY (Id),
	FOREIGN KEY (Intervention_Area_Test_Question_Id) REFERENCES Intervention_Area_Test_Question(Id));

CREATE TABLE Trial_Participant_Intervention_Area_Test (
	Id INT IDENTITY(1,1) NOT NULL,
	Trial_Participant_Id INT,
	Intervention_Area_Test_Id INT,
	DateTaken DateTime,
	PRIMARY KEY (Id),
	FOREIGN KEY (Trial_Participant_Id) REFERENCES Trial_Participant(Id),
	FOREIGN KEY (Intervention_Area_Test_Id) REFERENCES Intervention_Area_Test(Id));											

CREATE TABLE Intervention_Area_Test_Question_Answer (
	Id INT IDENTITY(1,1) NOT NULL,
	Intervention_Area_Test_Question_Id INT,
	Trial_Participant_Intervention_Area_Test_Id INT,
	Answer VARCHAR(MAX),
	Answer_Type INT,
	PRIMARY KEY (Id),
	FOREIGN KEY (Intervention_Area_Test_Question_Id) REFERENCES Intervention_Area_Test_Question(Id),
	FOREIGN KEY (Trial_Participant_Intervention_Area_Test_Id) REFERENCES Trial_Participant_Intervention_Area_Test(Id));

CREATE TABLE Participant_Group_Intervention_Area (
	Id INT IDENTITY(1,1) NOT NULL,
	Participant_Group_Id INT,
	Intervention_Area_Id INT,
	PRIMARY KEY (Id),
	FOREIGN KEY (Participant_Group_Id) REFERENCES Participant_Group(Id),
	FOREIGN KEY (Intervention_Area_Id) REFERENCES Intervention_Area(Id));

CREATE TABLE Investigator_Intervention_Area (
	Id INT IDENTITY(1,1) NOT NULL,
	Investigator_Id INT,
	Intervention_Area_Id INT,
	PRIMARY KEY (Id),
	FOREIGN KEY (Investigator_Id) REFERENCES Investigator(Id),
	FOREIGN KEY (Intervention_Area_Id) REFERENCES Intervention_Area(Id));

CREATE TABLE Assessment_Type (
	Id  INT IDENTITY(1,1) NOT NULL,
	Trial_Id INT,
	Name VARCHAR(1000),
	PRIMARY KEY (Id),
	FOREIGN KEY (Trial_Id) REFERENCES Trial(Id));

CREATE TABLE Trial_Participant_Assessment_Type (
	Id INT IDENTITY(1,1) NOT NULL,
	Trial_Participant_Id INT,
	Assessment_Type_Id INT,
	PRIMARY KEY (Id),
	FOREIGN KEY (Trial_Participant_Id) REFERENCES Trial_Participant(Id),
	FOREIGN KEY (Assessment_Type_Id) REFERENCES Assessment_Type(Id));

CREATE TABLE Assessment_Type_Question (
	Id INT IDENTITY(1,1) NOT NULL,
	Assessment_Type_Id INT,
	Question VARCHAR(1000),
	Question_Type INT,
	PRIMARY KEY (Id),
	FOREIGN KEY (Assessment_Type_Id) REFERENCES Assessment_Type(Id));

CREATE TABLE Assessment_Type_Option (
	Id INT IDENTITY(1,1) NOT NULL,
	Assessment_Type_Question_Id INT,
	Opt VARCHAR(MAX),
	PRIMARY KEY (Id),
	FOREIGN KEY (Assessment_Type_Question_Id) REFERENCES Assessment_Type_Question(Id));

CREATE TABLE Assessment_Type_Question_Answer (
	Id INT IDENTITY(1,1) NOT NULL,
	Assessment_Type_Question_Id INT,
	Trial_Participant_Assessment_Type_Id INT,
	Answer VARCHAR(MAX),
	Answer_Type INT,
	PRIMARY KEY (Id),
	FOREIGN KEY (Assessment_Type_Question_Id) REFERENCES Assessment_Type_Question(Id),
	FOREIGN KEY (Trial_Participant_Assessment_Type_Id) REFERENCES Trial_Participant_Assessment_Type(Id));
