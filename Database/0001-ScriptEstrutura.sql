IF NOT EXISTS (select 1 from dbo.sysobjects where id = object_id(N'dbo.tbRepositorio') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN

	CREATE TABLE [dbo].[tbRepositorio](
		[Id_repositorio] [int] IDENTITY(1,1) NOT NULL,
		[Nm_repositorio] [varchar](100) NOT NULL,
		[Nm_url_clone] [varchar](200) NOT NULL,
		[Nm_usuario] [varchar](50) NOT NULL,
		[Nm_senha] [varchar](50) NOT NULL,
		[Nm_email_admin] [varchar](50) NOT NULL,
		PRIMARY KEY (Id_repositorio)
		) 

END

GO

IF NOT EXISTS (select 1 from dbo.sysobjects where id = object_id(N'dbo.tbBranch') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN

	CREATE TABLE [dbo].[tbBranch](
		[Id_branch] [int] IDENTITY(1,1) NOT NULL,
		[Id_repositorio] [INT] NOT NULL,
		[Nm_branch] [varchar](200) NOT NULL,
		[Nm_email_dev] [varchar](50) NOT NULL,
		[Nm_email_review] [varchar](50) NOT NULL,
		PRIMARY KEY (Id_branch),
		FOREIGN KEY (Id_repositorio) REFERENCES tbRepositorio(Id_repositorio)
		)

END

IF NOT EXISTS (select 1 from dbo.sysobjects where id = object_id(N'dbo.tbSLA') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN

	CREATE TABLE [dbo].[tbSLA](
		[Id_SLA] [int] IDENTITY(1,1) NOT NULL,
		[Id_repositorio] [INT] NOT NULL,
		[Nr_dias_sla_commit] [int] NOT NULL,
		[Nr_dias_sla_review] [int] NOT NULL,
		FOREIGN KEY (Id_repositorio) REFERENCES tbRepositorio(Id_repositorio)
		)
END

GO

IF NOT EXISTS (select 1 from dbo.sysobjects where id = object_id(N'dbo.tbCommit') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN

	CREATE TABLE [dbo].[tbCommit](
		[Id_Commit] [varchar](50) NOT NULL,
		[Id_branch] [int] NOT NULL,
		[Nm_mensagem] [varchar](200) NOT NULL,
		[Nm_autor] [varchar](100) NOT NULL,
		[Dt_commit] [datetime] NOT NULL,
		FOREIGN KEY (Id_branch) REFERENCES tbBranch(Id_branch)
		)

END

GO

IF NOT EXISTS (select 1 from dbo.sysobjects where id = object_id(N'dbo.tbFeedback') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN

	CREATE TABLE [dbo].[tbFeedback](
		[Id_feedback] [int] IDENTITY(1,1) NOT NULL,
		[Id_commit] [varchar](50) NOT NULL,
		[Status_resposta] [varchar](50),
		[Mensagem_feedback][varchar](700),
		[Dt_registro][datetime] NOT NULL,
		[Dt_feedback][datetime],
		[Id_branch][int] not null
		PRIMARY KEY(Id_feedback)
		FOREIGN KEY (Id_branch) references tbBranch(Id_branch)
	)
	
END

GO

IF NOT EXISTS(SELECT TOP 1 1 FROM tbRepositorio (NOLOCK) WHERE Nm_repositorio = 'eSeg_Vida_1841')
BEGIN
	INSERT INTO tbRepositorio(Nm_repositorio,
							  Nm_url_clone,
							  Nm_usuario,
							  Nm_senha,
							  Nm_email_admin)
	VALUES ('eSeg_Vida_1841',
			'https://tfs.seniorsolution.com.br/Eseg/_git/eSeg_Vida_1841',
			'system.eseg',
			'$y$teME$eG',
			'vinicius.nogueira@sinqia.com.br'
	)
END

GO

IF NOT EXISTS(SELECT TOP 1 1 FROM tbBranch (NOLOCK) WHERE Nm_branch = 'Teste_CodeReview')
BEGIN

	DECLARE @Id_repositorio INT
	SELECT @Id_repositorio = Id_repositorio 
	FROM tbRepositorio (NOLOCK)
	WHERE Nm_repositorio = 'eSeg_Vida_1841' 

	INSERT INTO tbBranch (Id_repositorio,
						  Nm_branch,
						  Nm_email_dev,
						  Nm_email_review
						  )
	VALUES (@Id_repositorio,
			'Teste_CodeReview',
			'rodrigo.zanchetta@sinqia.com.br',
			'vinicius.nogueira@sinqia.com.br'
			)

END

GO

IF NOT EXISTS (SELECT TOP 1 1 FROM tbSLA (NOLOCK))
BEGIN

	DECLARE @Id_repo INT
	SELECT @Id_repo = Id_repositorio 
	FROM tbRepositorio (NOLOCK)
	WHERE Nm_repositorio = 'eSeg_Vida_1841' 

	INSERT INTO tbSLA (Id_repositorio,
					   Nr_dias_sla_commit,
					   Nr_dias_sla_review)
	VALUES (@Id_repo,
			5,
			2)		
END

GO

IF NOT EXISTS(SELECT TOP 1 1 FROM tbCommit (NOLOCK))
BEGIN

	DECLARE @Id_branch INT
	SELECT @Id_branch = Id_branch
	FROM tbBranch (NOLOCK)
	WHERE Nm_branch = 'Teste_CodeReview'

	INSERT INTO tbCommit(Id_Commit,
						 Id_branch,
						 Nm_mensagem,
						 Nm_autor,
						 Dt_commit)
	VALUES ('51029c05',
			@Id_branch,
			'Atualização de fonte 04/02/2022',
			'Vinicius Nogueira',
			'20220204'

	)

END