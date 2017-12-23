
if exists(select 1 from sysobjects where name = 'getTrainersFromMyLocationByDistance')
 drop proc getTrainersFromMyLocationByDistance
Go

create proc dbo.getTrainersFromMyLocationByDistance 
(
 @orig_lat  decimal(10,7),
 @orig_lon  decimal(10,7),
 @dist  decimal
)


AS
SELECT * FROM(
    SELECT *,(((acos(sin((@orig_lat*pi()/180)) * sin((locationlatitude*pi()/180))+cos((@orig_lat*pi()/180)) * cos((locationlatitude*pi()/180)) * cos(((@orig_lon - LocationLongitude)*pi()/180))))*180/pi())*60*1.1515*1.609344) as distance FROM aspnetusers) t
WHERE distance <= @dist 

GO

--getTrainersFromMyLocationByDistance -38.1062830 , 145.3484260, 1000
