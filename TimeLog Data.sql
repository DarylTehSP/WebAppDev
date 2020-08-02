USE TMSDB_1829316	
SELECT * FROM TimeLog
SELECT * FROM CustomerAccount
SELECT * FROM InstructorAccount
SELECT * FROM AppUser


DELETE FROM TimeLog
  WHERE Id>0;

SET IDENTITY_INSERT TimeLog ON 
INSERT TimeLog(Id, Date, ProjectId, HoursWorked, InstructorId)
VALUES 
('1', '2020/04/01','1','8','7'),
('2', '2020/04/02','1','8','7'),
('3', '2020/04/03','1','8','7'),
('4', '2020/04/04','1','8','7')

