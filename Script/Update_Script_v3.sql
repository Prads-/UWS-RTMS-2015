CREATE TABLE Participant_Intervention_Area_API_Key (
	Id INT IDENTITY(1,1) NOT NULL,
	Participant_Id INT,
	Intervention_Area_Id INT,
	API_Key VARCHAR(50),
	PRIMARY KEY (Id),
	FOREIGN KEY (Participant_Id) REFERENCES Participant(Id),
	FOREIGN KEY (Intervention_Area_Id) REFERENCES Intervention_Area(Id));
