drop sequence if exists public.ef_core_hilo_sequence;
CREATE SEQUENCE public.ef_core_hilo_sequence
    INCREMENT 100
    START 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

ALTER SEQUENCE public.ef_core_hilo_sequence
    OWNER TO postgres;

DROP TABLE if exists public.entries;

CREATE TABLE public.entries
(
    id integer NOT NULL,
    sub_id integer,
    title character varying(64) COLLATE pg_catalog."default" NOT NULL,
    text character varying(1024) COLLATE pg_catalog."default",
    CONSTRAINT entires_pkey PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.entries
    OWNER to postgres;
