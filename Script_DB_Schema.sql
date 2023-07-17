GO
/****** Object:  User [usrALVerificaFirme]    Script Date: 28/04/2023 16:57:52 ******/
CREATE USER [usrVerificaFirme] FOR LOGIN [usrVerificaFirme] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [usrVerificaFirme]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [usrVerificaFirme]
GO
/****** Object:  UserDefinedFunction [dbo].[fnModulo_Stato]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  Table [dbo].[CategorieEsclusione]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CategorieEsclusione](
	[Cod] [varchar](3) NOT NULL,
	[Descrizione] [varchar](250) NOT NULL,
	[DescrizioneBreve] [varchar](100) NOT NULL,
 CONSTRAINT [PK_CategorieEsclusione] PRIMARY KEY CLUSTERED 
(
	[Cod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CategorieSanabilita]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CategorieSanabilita](
	[Cod] [varchar](3) NOT NULL,
	[Descrizione] [varchar](250) NOT NULL,
	[DescrizioneBreve] [varchar](100) NOT NULL,
 CONSTRAINT [PK_CategorieSanabilita] PRIMARY KEY CLUSTERED 
(
	[Cod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Modulo]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Modulo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IDProcedimento] [int] NOT NULL,
	[CodicePostazione] [varchar](3) NOT NULL,
	[Numero] [int] NOT NULL,
	[IsNullo] [bit] NOT NULL,
	[NumeroRighe] [int] NOT NULL,
	[CodComuneListaElettorale] [varchar](6) NULL,
	[CodCategoriaEsclusione] [varchar](3) NULL,
	[IsCompleto] [bit] NOT NULL,
	[UsernameCreazione] [varchar](50) NOT NULL,
	[DataOraCreazione] [datetime] NOT NULL,
	[UsernameModifica] [varchar](50) NULL,
	[DataOraModifica] [datetime] NULL,
 CONSTRAINT [PK_Modulo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ModuloNominativo]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ModuloNominativo](
	[IDModulo] [int] NOT NULL,
	[NumeroRiga] [int] NOT NULL,
	[Nome] [nvarchar](100) NULL,
	[Cognome] [nvarchar](100) NULL,
	[DataNascita] [date] NULL,
	[CodComuneNascita] [varchar](6) NULL,
	[CodComuneListaElettorale] [varchar](6) NULL,
	[NListaElettorale] [nvarchar](50) NULL,
	[CodCategorieEsclusione] [varchar](3) NULL,
	[CodCategorieSanabilita] [varchar](3) NULL,
	[Note] [nvarchar](256) NULL,
 CONSTRAINT [PK_ModuloNominativo_1] PRIMARY KEY CLUSTERED 
(
	[IDModulo] ASC,
	[NumeroRiga] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ModuloNominativoLog]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ModuloNominativoLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IDModuloNominativo] [int] NOT NULL,
	[NumeroRiga] [int] NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[DataOraModifica] [datetime] NOT NULL,
 CONSTRAINT [PK_ModuloNominativoLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Parametri]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Parametri](
	[Cod] [varchar](20) NOT NULL,
	[Descrizione] [varchar](100) NOT NULL,
	[Valore] [varchar](250) NOT NULL,
 CONSTRAINT [PK_Parametri] PRIMARY KEY CLUSTERED 
(
	[Cod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Procedimento]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Procedimento](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CodStato] [varchar](3) NOT NULL,
	[Descrizione] [nvarchar](250) NOT NULL,
	[QuorumFirme] [int] NOT NULL,
	[NumeroPostazioni] [int] NOT NULL,
	[NumeroModuli] [int] NOT NULL,
	[AnnoFirmatario] [int] NOT NULL,
	[UsernameCreazione] [varchar](50) NOT NULL,
	[DataOraCreazione] [datetime] NOT NULL,
	[UsernameModifica] [varchar](50) NOT NULL,
	[DataOraModifica] [datetime] NOT NULL,
 CONSTRAINT [PK_Procedimento] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProcedimentoPostazione]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProcedimentoPostazione](
	[IDProcedimento] [int] NOT NULL,
	[CodicePostazione] [varchar](3) NOT NULL,
	[ModuloDa] [int] NOT NULL,
	[ModuloA] [int] NOT NULL,
 CONSTRAINT [PK_ProcedimentoPostazione] PRIMARY KEY CLUSTERED 
(
	[IDProcedimento] ASC,
	[CodicePostazione] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Profilo]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Profilo](
	[ID] [int] NOT NULL,
	[Descrizione] [varchar](50) NULL,
 CONSTRAINT [PK_Profilo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Stato]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stato](
	[Cod] [varchar](3) NOT NULL,
	[Descrizione] [varchar](100) NOT NULL,
 CONSTRAINT [PK_Stato] PRIMARY KEY CLUSTERED 
(
	[Cod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Utente]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Utente](
	[Username] [varchar](50) NOT NULL,
	[Nome] [varchar](100) NOT NULL,
	[Cognome] [varchar](100) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[IsAttivo] [bit] NOT NULL,
 CONSTRAINT [PK_Utente] PRIMARY KEY CLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UtenteProcedimento]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UtenteProcedimento](
	[Username] [varchar](50) NOT NULL,
	[IDProcedimento] [int] NOT NULL,
 CONSTRAINT [PK_UtenteProcedimento] PRIMARY KEY CLUSTERED 
(
	[Username] ASC,
	[IDProcedimento] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UtenteProfilo]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UtenteProfilo](
	[Username] [varchar](50) NOT NULL,
	[IDProfilo] [int] NOT NULL,
	[ValidoDal] [datetime] NOT NULL,
	[ValidoAl] [datetime] NULL,
 CONSTRAINT [PK_UtenteProfilo] PRIMARY KEY CLUSTERED 
(
	[Username] ASC,
	[IDProfilo] ASC,
	[ValidoDal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Comuni](
	[cod_reg] [varchar](2) NULL,
	[cod_istat] [varchar](6) NOT NULL,
	[sigla_prov] [varchar](2) NULL,
	[des_com] [varchar](45) NULL,
 CONSTRAINT [PK_comuni_1__17] PRIMARY KEY NONCLUSTERED 
(
	[cod_istat] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Province](
	[cod_reg] [varchar](2) NOT NULL,
	[cod_prov] [varchar](3) NOT NULL,
	[sigla_prov] [varchar](2) NOT NULL,
	[des_prov] [varchar](22) NOT NULL,
	[des_reg] [varchar](20) NOT NULL,
 CONSTRAINT [PK_Province] PRIMARY KEY CLUSTERED 
(
	[cod_prov] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO




CREATE TABLE [dbo].[Regioni](
	[Cod] [varchar](2) NOT NULL,
	[Descrizione] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Regioni] PRIMARY KEY NONCLUSTERED 
(
	[cod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO




ALTER TABLE [dbo].[Modulo]  WITH CHECK ADD  CONSTRAINT [FK_Modulo_CategorieEsclusione] FOREIGN KEY([CodCategoriaEsclusione])
REFERENCES [dbo].[CategorieEsclusione] ([Cod])
GO
ALTER TABLE [dbo].[Modulo] CHECK CONSTRAINT [FK_Modulo_CategorieEsclusione]
GO
ALTER TABLE [dbo].[Modulo]  WITH CHECK ADD  CONSTRAINT [FK_Modulo_ProcedimentoPostazione] FOREIGN KEY([IDProcedimento], [CodicePostazione])
REFERENCES [dbo].[ProcedimentoPostazione] ([IDProcedimento], [CodicePostazione])
GO
ALTER TABLE [dbo].[Modulo] CHECK CONSTRAINT [FK_Modulo_ProcedimentoPostazione]
GO
ALTER TABLE [dbo].[ModuloNominativo]  WITH CHECK ADD  CONSTRAINT [FK_ModuloNominativo_CategorieEsclusione] FOREIGN KEY([CodCategorieEsclusione])
REFERENCES [dbo].[CategorieEsclusione] ([Cod])
GO
ALTER TABLE [dbo].[ModuloNominativo] CHECK CONSTRAINT [FK_ModuloNominativo_CategorieEsclusione]
GO
ALTER TABLE [dbo].[ModuloNominativo]  WITH CHECK ADD  CONSTRAINT [FK_ModuloNominativo_CategorieSanabilita] FOREIGN KEY([CodCategorieSanabilita])
REFERENCES [dbo].[CategorieSanabilita] ([Cod])
GO
ALTER TABLE [dbo].[ModuloNominativo] CHECK CONSTRAINT [FK_ModuloNominativo_CategorieSanabilita]
GO
ALTER TABLE [dbo].[ModuloNominativo]  WITH CHECK ADD  CONSTRAINT [FK_ModuloNominativo_Modulo] FOREIGN KEY([IDModulo])
REFERENCES [dbo].[Modulo] ([ID])
GO
ALTER TABLE [dbo].[ModuloNominativo] CHECK CONSTRAINT [FK_ModuloNominativo_Modulo]
GO
ALTER TABLE [dbo].[ModuloNominativoLog]  WITH CHECK ADD  CONSTRAINT [FK_ModuloNominativoLog_ModuloNominativo] FOREIGN KEY([IDModuloNominativo], [NumeroRiga])
REFERENCES [dbo].[ModuloNominativo] ([IDModulo], [NumeroRiga])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ModuloNominativoLog] CHECK CONSTRAINT [FK_ModuloNominativoLog_ModuloNominativo]
GO
ALTER TABLE [dbo].[Procedimento]  WITH CHECK ADD  CONSTRAINT [FK_Procedimento_Stato] FOREIGN KEY([CodStato])
REFERENCES [dbo].[Stato] ([Cod])
GO
ALTER TABLE [dbo].[Procedimento] CHECK CONSTRAINT [FK_Procedimento_Stato]
GO
ALTER TABLE [dbo].[ProcedimentoPostazione]  WITH CHECK ADD  CONSTRAINT [FK_ProcedimentoPostazione_Procedimento] FOREIGN KEY([IDProcedimento])
REFERENCES [dbo].[Procedimento] ([ID])
GO
ALTER TABLE [dbo].[ProcedimentoPostazione] CHECK CONSTRAINT [FK_ProcedimentoPostazione_Procedimento]
GO
ALTER TABLE [dbo].[UtenteProcedimento]  WITH CHECK ADD  CONSTRAINT [FK_UtenteProcedimento_Procedimento] FOREIGN KEY([IDProcedimento])
REFERENCES [dbo].[Procedimento] ([ID])
GO
ALTER TABLE [dbo].[UtenteProcedimento] CHECK CONSTRAINT [FK_UtenteProcedimento_Procedimento]
GO
ALTER TABLE [dbo].[UtenteProcedimento]  WITH CHECK ADD  CONSTRAINT [FK_UtenteProcedimento_Utente] FOREIGN KEY([Username])
REFERENCES [dbo].[Utente] ([Username])
GO
ALTER TABLE [dbo].[UtenteProcedimento] CHECK CONSTRAINT [FK_UtenteProcedimento_Utente]
GO
ALTER TABLE [dbo].[UtenteProfilo]  WITH CHECK ADD  CONSTRAINT [FK_UtenteProfilo_Profilo] FOREIGN KEY([IDProfilo])
REFERENCES [dbo].[Profilo] ([ID])
GO
ALTER TABLE [dbo].[UtenteProfilo] CHECK CONSTRAINT [FK_UtenteProfilo_Profilo]
GO
ALTER TABLE [dbo].[UtenteProfilo]  WITH CHECK ADD  CONSTRAINT [FK_UtenteProfilo_Utente] FOREIGN KEY([Username])
REFERENCES [dbo].[Utente] ([Username])
GO
ALTER TABLE [dbo].[UtenteProfilo] CHECK CONSTRAINT [FK_UtenteProfilo_Utente]
GO
/****** Object:  StoredProcedure [dbo].[Procedimento_Elimina]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fnModulo_Stato]
(
	@IDModulo int
)
RETURNS varchar(100)
AS
BEGIN
	DECLARE @Stato varchar(100)

	declare @conta int = (select count(*) from [dbo].[ModuloNominativo] where IDModulo = @IDModulo
							and 
								(nome is not null or
								 Cognome is not null or
								 DataNascita is not null or
								 CodComuneListaElettorale is not null or
								 CodComuneNascita is not null or
								 CodCategorieEsclusione is not null))

	declare @isCompleto bit = (select [IsCompleto] from [dbo].[Modulo] where ID = @IDModulo)
	declare @isNullo bit = (select IsNullo from [dbo].[Modulo] where ID = @IDModulo)
	declare @numeroRighe int = (select NumeroRighe from [dbo].[Modulo] where ID = @IDModulo)

	if (@isCompleto = 1)
		begin
			SET @Stato = 'Completato' + (case when @isNullo = 1 then ' (modulo NULLO)' else '' end)
		end
	else
		begin
			if (@conta <= @numeroRighe)
				begin
					SET @Stato = 'In lavorazione'
				end
			if (@conta = 0)
				begin
					SET @Stato = 'Creato'
				end
		end

	RETURN @Stato

END
GO
/****** Object:  View [dbo].[vvComune]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[vvComune]
AS
	SELECT 
		[cod_reg],
		[cod_istat],
		[sigla_prov],
		[des_com],
		upper([des_com]) + ' (' +upper([sigla_prov])+')' DescrizioneCompleta
	FROM 
		[dbo].[Comuni]

	UNION
		SELECT 
		'',
		'NT',
		'',
		'NON TROVATO',
		'NON TROVATO' DescrizioneCompleta
	
	UNION
		SELECT 
		'',
		'E',
		'',
		'ESTERO',
		'ESTERO' DescrizioneCompleta
GO


CREATE VIEW [dbo].[vvProvincia]
AS
	SELECT 
		cod_reg, 
		cod_prov,
		sigla_prov,
		des_prov,
		des_reg
	FROM 
		[dbo].[Province]

	
GO


CREATE PROCEDURE [dbo].[Procedimento_Elimina]
	@IdProcedimento INT
AS
BEGIN
	DELETE FROM [dbo].[ProcedimentoPostazione]
	WHERE IDProcedimento = @IdProcedimento

	DELETE FROM [dbo].[UtenteProcedimento]
	WHERE IDProcedimento = @IdProcedimento

	DELETE FROM [dbo].[Procedimento]
	WHERE ID = @IdProcedimento

END
GO
/****** Object:  StoredProcedure [dbo].[Report_NominativiDuplicatiPerProcedimento]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Report_NominativiDuplicatiPerProcedimento]
	@IdProcedimento INT,
	@CodComuneNascita varchar(6) = NULL,
	@CodComuneListaElettorale varchar(6) = NULL,
	@AncheEsclusi bit = 0
AS
BEGIN

select  RANK() OVER (order by mn.Cognome, mn.Nome, mn.DataNascita, mn.CodComuneNascita) as Raggruppamento,
		mn.NumeroRiga,
		mn.IDModulo,
		m.CodicePostazione,
		m.Numero,
		mn.Cognome, 
		mn.Nome,
		mn.CodComuneNascita,
		(select des_com from vvComune where cod_istat = mn.CodComuneNascita) DescComuneNascita,
		mn.CodComuneListaElettorale,
		(select des_com from vvComune where cod_istat = mn.CodComuneListaElettorale) DescComuneListaElettorale,
		mn.DataNascita,
		mn.CodCategorieEsclusione,
		mn.Note
		from ModuloNominativo mn
join Modulo m on m.id = mn.IDModulo
where m.IDProcedimento = @IdProcedimento
and (@CodComuneNascita IS NULL OR mn.CodComuneNascita = @CodComuneNascita)
		and (@CodComuneListaElettorale IS NULL OR  mn.CodComuneListaElettorale = @CodComuneListaElettorale)
and (mn.CodCategorieEsclusione is null OR @AncheEsclusi = 1)
and (select count(*) 
		from ModuloNominativo mn4
		join Modulo m4 on mn4.IDModulo = m4.ID
		where m4.IDProcedimento = @IdProcedimento
		and mn.Nome = mn4.Nome
		and mn.Cognome = mn4.Cognome
		and mn.DataNascita = mn4.DataNascita
		and mn.CodComuneNascita = mn4.CodComuneNascita
		and (mn4.CodCategorieEsclusione is null OR @AncheEsclusi = 1)) > 1

order by mn.Cognome, Mn.Nome, Raggruppamento
END
GO
/****** Object:  StoredProcedure [dbo].[Report_RiepilogoDataEntry]    Script Date: 28/04/2023 16:57:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Report_RiepilogoDataEntry]
	@IdProcedimento INT,
	@TipoRaggruppamento INT --1 modulo, 2 postazione, 3 provincia, 4 comune
AS
BEGIN
	SET NOCOUNT ON;
		IF (@TipoRaggruppamento = 1)
			BEGIN
				SELECT 
					p.Descrizione,
					p.QuorumFirme,
					m.CodicePostazione,
					convert(varchar(10), m.Numero) as NumeroModulo, 
					m.id as IdModulo,
					(SELECT COUNT(*) FROM ModuloNominativo mn inner join modulo m1 on mn.IDModulo = m1.ID  where mn.IDModulo = m.ID 
									and mn.Nome is not null and mn.Cognome is not null 
									and mn.CodComuneListaElettorale is not null and mn.CodComuneNascita is not null
									and mn.DataNascita is not null and
									mn.CodCategorieEsclusione is null and mn.CodCategorieSanabilita is null and
									m1.IsNullo = 0) as RecordValidi,
					(SELECT COUNT(*) FROM ModuloNominativo mn where mn.IDModulo = m.ID and mn.CodCategorieEsclusione is not null and mn.CodCategorieSanabilita is null) as RecordNulli,
					(SELECT COUNT(*) FROM ModuloNominativo mn where mn.IDModulo = m.ID and mn.CodCategorieSanabilita is not null) as RecordSanabili,
					(SELECT COUNT(*) FROM ModuloNominativo mn where mn.IDModulo = m.ID) as RecordTotali,
					(SELECT COUNT(*) FROM ModuloNominativo mn inner join modulo m1 on mn.IDModulo = m1.ID where mn.IDModulo = m.ID and 
										m1.IsCompleto = 1 and m1.IsNullo = 0 and mn.CodCategorieEsclusione is null
										and mn.Nome is not null and mn.Cognome is not null 
										and mn.CodComuneListaElettorale is not null and mn.CodComuneNascita is not null
										and mn.DataNascita is not null) as Firme,
					(SELECT [dbo].[fnModulo_Stato](m.ID)) Stato
				FROM
					dbo.Procedimento p 
					INNER JOIN dbo.Modulo m on p.ID = m.IDProcedimento
				WHERE 
					p.ID = @IdProcedimento	
				ORDER BY
					m.CodicePostazione,
					m.Numero
			END
		IF (@TipoRaggruppamento = 2)
			BEGIN
				SELECT 
					max(p.Descrizione) Descrizione,
					max(p.QuorumFirme) QuorumFirme,
					m.CodicePostazione,
					null as NumeroModulo, 
					null as IdModulo,
					(sum(case when	mn.Nome is not null and 
								mn.Cognome is not null and
								mn.CodComuneListaElettorale is not null and 
								mn.CodComuneNascita is not null and
								mn.DataNascita is not null and
								mn.CodCategorieEsclusione is null and 
								mn.CodCategorieSanabilita is null and
								m.IsNullo = 0
							then 1
							else 0
					 end)) as RecordValidi,
					(sum(case when	mn.CodCategorieEsclusione is not null and 
								mn.CodCategorieSanabilita is null
							then 1
							else 0
					 end)) as RecordNulli,
					(sum(case when mn.CodCategorieSanabilita is not null then 1 else 0 end)) as RecordSanabili,
					(sum(case when mn.IDModulo is not null then 1 else 0 end)) as RecordTotali,
					(sum(case when	m.IsCompleto = 1 and 
								m.IsNullo = 0 and 
								mn.CodCategorieEsclusione is null and
								mn.Nome is not null and mn.Cognome is not null and
								mn.CodComuneListaElettorale is not null and 
								mn.CodComuneNascita is not null and
								mn.DataNascita is not null
							then 1
							else 0
						end)) as Firme,
					null Stato
				FROM
					dbo.Procedimento p 
					INNER JOIN dbo.Modulo m on p.ID = m.IDProcedimento
					INNER JOIN dbo.ModuloNominativo mn on m.id = mn.[IDModulo]
				WHERE 
					p.ID = @IdProcedimento	
				GROUP BY
					m.CodicePostazione
				ORDER BY
					m.CodicePostazione
			END
		IF (@TipoRaggruppamento = 3)
			BEGIN
				SELECT 
					max(p.Descrizione) Descrizione,
					max(p.QuorumFirme) QuorumFirme,
					case when vp.[des_prov] IS NULL OR vp.[des_prov] = '' then '' else vp.[des_prov] end CodicePostazione,
					null as NumeroModulo, 
					null as IdModulo,
					(sum(case when	mn.Nome is not null and 
								mn.Cognome is not null and
								mn.CodComuneListaElettorale is not null and 
								mn.CodComuneNascita is not null and
								mn.DataNascita is not null and
								mn.CodCategorieEsclusione is null and 
								mn.CodCategorieSanabilita is null and
								m.IsNullo = 0
							then 1
							else 0
					 end)) as RecordValidi,
					(sum(case when	mn.CodCategorieEsclusione is not null and 
								mn.CodCategorieSanabilita is null
							then 1
							else 0
					 end)) as RecordNulli,
					(sum(case when mn.CodCategorieSanabilita is not null then 1 else 0 end)) as RecordSanabili,
					(sum(case when mn.IDModulo is not null then 1 else 0 end)) as RecordTotali,
					(sum(case when	m.IsCompleto = 1 and 
								m.IsNullo = 0 and 
								mn.CodCategorieEsclusione is null and
								mn.Nome is not null and mn.Cognome is not null and
								mn.CodComuneListaElettorale is not null and 
								mn.CodComuneNascita is not null and
								mn.DataNascita is not null
							then 1
							else 0
						end)) as Firme,
					null Stato
				FROM
					dbo.Procedimento p 
					INNER JOIN dbo.Modulo m on p.ID = m.IDProcedimento
					INNER JOIN dbo.ModuloNominativo mn on m.id = mn.[IDModulo]
					LEFT JOIN [dbo].[vvComune] vc on mn.CodComuneListaElettorale = vc.cod_istat
					LEFT JOIN [dbo].[vvProvincia] vp on vc.sigla_prov = vp.[sigla_prov]
				WHERE 
					p.ID = @IdProcedimento	
				GROUP BY
					case when vp.[des_prov] IS NULL OR vp.[des_prov] = '' then '' else vp.[des_prov] end
				ORDER BY
					case when vp.[des_prov] IS NULL OR vp.[des_prov] = '' then '' else vp.[des_prov] end
			END
		IF (@TipoRaggruppamento = 4)
			BEGIN
				SELECT 
					max(p.Descrizione) Descrizione,
					max(p.QuorumFirme) QuorumFirme,
					case when vp.[des_prov] IS NULL OR vp.[des_prov] = '' then 'zzz' else vp.[des_prov] end CodicePostazione,
					case when vc.des_com IS NULL OR vc.des_com = '' then 'zzz' else vc.des_com end as NumeroModulo, 
					null as IdModulo,
					(sum(case when	mn.Nome is not null and 
								mn.Cognome is not null and
								mn.CodComuneListaElettorale is not null and 
								mn.CodComuneNascita is not null and
								mn.DataNascita is not null and
								mn.CodCategorieEsclusione is null and 
								mn.CodCategorieSanabilita is null and
								m.IsNullo = 0
							then 1
							else 0
					 end)) as RecordValidi,
					(sum(case when	mn.CodCategorieEsclusione is not null and 
								mn.CodCategorieSanabilita is null
							then 1
							else 0
					 end)) as RecordNulli,
					(sum(case when mn.CodCategorieSanabilita is not null then 1 else 0 end)) as RecordSanabili,
					(sum(case when mn.IDModulo is not null then 1 else 0 end)) as RecordTotali,
					(sum(case when	m.IsCompleto = 1 and 
								m.IsNullo = 0 and 
								mn.CodCategorieEsclusione is null and
								mn.Nome is not null and mn.Cognome is not null and
								mn.CodComuneListaElettorale is not null and 
								mn.CodComuneNascita is not null and
								mn.DataNascita is not null
							then 1
							else 0
						end)) as Firme,
					null Stato
				FROM
					dbo.Procedimento p 
					INNER JOIN dbo.Modulo m on p.ID = m.IDProcedimento
					INNER JOIN dbo.ModuloNominativo mn on m.id = mn.[IDModulo]
					LEFT JOIN [dbo].[vvComune] vc on mn.CodComuneListaElettorale = vc.cod_istat
					LEFT JOIN [dbo].[vvProvincia] vp on vc.sigla_prov = vp.[sigla_prov]
				WHERE 
					p.ID = @IdProcedimento	
				GROUP BY
					case when vp.[des_prov] IS NULL OR vp.[des_prov] = '' then 'zzz' else vp.[des_prov] end,
					case when vc.des_com IS NULL OR vc.des_com = '' then 'zzz' else vc.des_com end
				ORDER BY
					case when vp.[des_prov] IS NULL OR vp.[des_prov] = '' then 'zzz' else vp.[des_prov] end,
					case when vc.des_com IS NULL OR vc.des_com = '' then 'zzz' else vc.des_com end
			END

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Report_VerificaMinori]
	@IdProcedimento INT,
	@DataRiferimento date

AS
BEGIN

	declare @anno int = (select [AnnoFirmatario] from [dbo].[Procedimento] where id = @IdProcedimento)

	SELECT
		mn.NumeroRiga,
		mn.IDModulo,
		m.CodicePostazione,
		m.Numero,
		mn.Cognome, 
		mn.Nome,
		mn.CodComuneNascita,
		(select des_com from vvComune where cod_istat = mn.CodComuneNascita) DescComuneNascita,
		mn.CodComuneListaElettorale,
		(select des_com from vvComune where cod_istat = mn.CodComuneListaElettorale) DescComuneListaElettorale,
		mn.DataNascita,
		mn.CodCategorieEsclusione,
		mn.Note
	FROM
		dbo.ModuloNominativo mn
		INNER JOIN Modulo m on m.id = mn.IDModulo
	WHERE
		m.IDProcedimento = @IdProcedimento and
		(
			(@DataRiferimento is null and year(mn.DataNascita) >= @anno) or 
			(@DataRiferimento is not null and mn.DataNascita >= @DataRiferimento)
		)
	ORDER BY
		mn.Cognome, Mn.Nome
END



GRANT EXECUTE ON OBJECT::[dbo].[Procedimento_Elimina]
    TO usrVerificaFirme; 

GO

GRANT EXECUTE ON OBJECT::[dbo].[Report_NominativiDuplicatiPerProcedimento]
    TO usrVerificaFirme; 

GO

GRANT EXECUTE ON OBJECT::[dbo].[Report_RiepilogoDataEntry]
    TO usrVerificaFirme; 

GRANT EXECUTE ON OBJECT::[dbo].[Report_VerificaMinori]
    TO usrVerificaFirme; 
