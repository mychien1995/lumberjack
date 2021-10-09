USE [LSDB]
GO
/****** Object:  StoredProcedure [dbo].[AutoScale]    Script Date: 10/9/2021 12:37:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AutoScale_2]
(
	@currentTime bigint,
	@maximumSize bigint,
	@duration bigint,
	@preAllocate bigint
)
AS
BEGIN
	DECLARE @currentMaxTimestamp bigint;
	DECLARE @newTableName varchar(200);
	DECLARE @createTableQuery nvarchar(MAX);
	DECLARE @targetTimestamp bigint;
	SET @targetTimestamp = @currentTime + @preAllocate - @duration;
	IF(EXISTS(SELECT TOP 1 * FROM dbo.Shards))
	BEGIN
		SET @currentMaxTimestamp = (SELECT MAX(EndTimestamp) FROM dbo.Shards);
		IF(@currentMaxTimestamp < @targetTimestamp)
		BEGIN
			WHILE(@currentMaxTimestamp < @targetTimestamp)
			BEGIN
				PRINT @targetTimestamp
				SET @newTableName = 'dbo.LogDatas_' + CAST(@currentMaxTimestamp AS varchar(20)) + '_' + CAST((@currentMaxTimestamp + @duration) AS varchar(20))
				EXEC dbo.CreateLogTable @TableName = @newTableName;
				INSERT INTO Shards (ShardId, Number, TableName,StartTimestamp, EndTimestamp, MaximumSize, CreatedDate)
				VALUES(NEWID() , 1, @newTableName, @currentTime, @currentMaxTimestamp + @duration, @maximumSize, @currentTime);
				SET @currentMaxTimestamp = (SELECT MAX(EndTimestamp) FROM dbo.Shards);
			END
		END
	END
	ELSE
	BEGIN
		SET @currentMaxTimestamp = @currentTime + @duration;
		SET @newTableName = 'dbo.LogDatas_' + CAST(@currentTime AS varchar(20)) + '_' + CAST(@currentMaxTimestamp AS varchar(20))
		EXEC dbo.CreateLogTable @TableName = @newTableName;
		INSERT INTO Shards (ShardId, Number, TableName,StartTimestamp, EndTimestamp, MaximumSize, CreatedDate)
		VALUES(NEWID() , 1, @newTableName, @currentTime, @currentMaxTimestamp, @maximumSize, @currentTime);
		EXEC dbo.AutoScale @currentTime, @maximumSize, @duration, @preAllocate
	END
END
