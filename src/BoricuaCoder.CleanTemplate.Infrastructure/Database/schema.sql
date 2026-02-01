CREATE TABLE IF NOT EXISTS movies (
    id          UUID PRIMARY KEY,
    title       VARCHAR(200)    NOT NULL,
    year        INT             NOT NULL,
    imdb_id     VARCHAR(20)     NULL,
    CONSTRAINT uq_movies_title_year UNIQUE (title, year)
);
