import http from 'k6/http';
import { sleep } from 'k6';
import { check } from 'k6';

const API_URL = 'https://localhost:7080/api';
const AUTHTOKEN = '123';

export const options = {
  vus: 50,
  duration: '30s',
  ext: {
    loadimpact: {
      projectID: 3564688,
      // Test runs with the same name groups test runs together
      name: 'Testing signup user',
    },
  },
};

export default function () {
  const params = { headers: { Authorization: `Bearer ${AUTHTOKEN}` } };

  let createUserResponse = http.post(`${API_URL}/user`, null, params);

  check(createUserResponse, { 'Create user response status code is 200': (r) => r.status == 200 });
}
