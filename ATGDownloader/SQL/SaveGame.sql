-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE SaveGame
	@atgGameId NVARCHAR(50)
	,@code NVARCHAR(50)
	,@date DATE
	,@track1 INT
	,@track2 INT = NULL
	,@status NVARCHAR(50)
	,@timestamp DATETIME
	,@version BIGINT = NULL
	,@jsonData NVARCHAR(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @gameId INT = 
	(
		SELECT TOP 1 [Id]
		FROM [dbo].[GameInfo]
		WHERE [Code] = @code
			AND [Date] = @date
			AND [Track1] = @track1
	)

	IF @gameId IS NULL
	BEGIN

		INSERT [dbo].[GameInfo]([ATGGameId], Code, Date, Track1, Track2)
		SELECT @atgGameId, @code, @date, @track1, @track2

		SET @gameId = SCOPE_IDENTITY()

	END

	INSERT [dbo].[GameData](GameId, Status, Timestamp, Version, JsonData)
	SELECT @gameId, @status, @timestamp, @version, @jsonData
	
	SELECT @gameId;

END
GO
