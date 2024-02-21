CREATE TABLE IF NOT EXISTS public.Clientes (
    id integer NOT NULL GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    saldo integer NOT NULL,
    limite integer NOT NULL
);

CREATE TABLE IF NOT EXISTS public.Transacoes (
    id SERIAL,
    valor integer NOT NULL,
    tipo smallint NOT NULL,
    descricao varchar NOT NULL,
    realizadoEm timestamp NOT NULL,
    clienteId integer REFERENCES Clientes(id)
);

CREATE INDEX index_clienteId_transacoes ON public.Transacoes
(
    clienteId ASC
);

INSERT INTO public.Clientes (saldo, limite) VALUES
	 (0, 100000),
	 (0, 80000),
	 (0, 1000000),
	 (0, 10000000),
	 (0, 500000);