DROP TABLE IF EXISTS tmdb;
DROP TABLE IF EXISTS common;
DROP TABLE IF EXISTS results;
DROP TABLE IF EXISTS ratings;

DROP SEQUENCE IF EXISTS result_seq;

CREATE TABLE common(
    thing_id INT CONSTRAINT PK_thing_id PRIMARY KEY,
    title NVARCHAR (64) NOT NULL,
    vote_count INT NOT NULL,
    vote_average FLOAT NOT NULL,
);

CREATE TABLE tmdb(
    thing_id INT CONSTRAINT FK_tmdb_thing_id FOREIGN KEY (thing_id) REFERENCES common(thing_id) NOT NULL,
    overview NVARCHAR (2048) NOT NULL,
    release_date DATETIME2 NOT NULL,
    popularity FLOAT NOT NULL,
    adult BIT NOT NULL,
    genre_ids NVARCHAR (256) NOT NULL,
    poster_path NVARCHAR (256) NOT NULL
);

CREATE TABLE ratings(
    rating_id UNIQUEIDENTIFIER CONSTRAINT PK_rating_id PRIMARY KEY,
    rating_name NVARCHAR (64) NOT NULL
);

CREATE SEQUENCE result_seq AS INT START WITH 1 increment BY 1; 
CREATE TABLE results(
    id INT CONSTRAINT PK_id PRIMARY KEY CONSTRAINT PK_result_id DEFAULT NEXT VALUE FOR result_seq,
    thing_id INT CONSTRAINT FK_result_thing_id FOREIGN KEY (thing_id) REFERENCES common(thing_id) NOT NULL,
    rating_id UNIQUEIDENTIFIER CONSTRAINT FK_rating_id FOREIGN KEY (rating_id) REFERENCES ratings(rating_id) NOT NULL,
    rating_value FLOAT NOT NULL,
);

