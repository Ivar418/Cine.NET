CREATE TABLE movies
(
    id       INT AUTO_INCREMENT PRIMARY KEY,
    title    VARCHAR(100) NOT NULL,
    tmdb_id   VARCHAR(255) NOT NULL UNIQUE,
    language VARCHAR(255),
    poster_url   VARCHAR(255),
    runtime  INT,
    auditorium VARCHAR(255),
    imdb_id   VARCHAR(255),
    release_date VARCHAR(10),
    about    VARCHAR(255),
    age_indication VARCHAR(255),
    spoken_language_name VARCHAR(255),
    spoken_language_code_iso_639_1 VARCHAR(255),
    genres VARCHAR(255)
);