create table Emp(
	EmpId int primary key,
	EmpName varchar(50),
	EmpCity varchar(50),
	EmpState varchar(50),
	DeptId int,
	Foreign Key (DeptId) references Dept(DeptId)
)
insert into Emp values (1,'Lalit','Hyderabad','Telangana',1) 
Create table Dept(
	DeptId int primary key,
	DeptName varchar(50),
	DeptLocation varchar(50),
	DeptJoining varchar(50),
	DeptLeaving varchar(50)
)

SELECT e.EmpId,e.EmpName,e.EmpCity,e.EmpState,d.DeptId
FROM Emp e
INNER JOIN Dept d ON Emp.DeptId = Dept.DeptId;