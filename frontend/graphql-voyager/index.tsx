import express from 'express';
import { express as voyagerMiddleware } from 'graphql-voyager/middleware';

const app = express();

app.use('/voyager', voyagerMiddleware({ endpointUrl: '/graphql/' }));

app.listen(80);
