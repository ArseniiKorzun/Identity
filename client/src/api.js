const API_URL = import.meta.env.VITE_API_URL ?? '/api'

async function request(path, options = {}) {
  let response
  try {
    response = await fetch(`${API_URL}${path}`, {
      ...options,
      headers: { 'Content-Type': 'application/json', ...options.headers },
    })
  } catch {
    throw new Error('Server is unavailable. Make sure the API is running.')
  }

  const text = await response.text()
  let data = text
  try {
    data = text ? JSON.parse(text) : null
  } catch {
    /* plain-text response */
  }

  if (!response.ok) throw new Error(extractError(data) || `Request failed (${response.status})`)
  return data
}

function extractError(data) {
  if (!data) return null
  if (typeof data === 'string') return data
  if (Array.isArray(data)) return data.map((e) => e.errorMessage).join(' ')
  if (data.errors) return Object.values(data.errors).flat().join(' ')
  return data.title || data.detail || null
}

const post = (path, body) => request(path, { method: 'POST', body: JSON.stringify(body) })
const put = (path, body) => request(path, { method: 'PUT', body: JSON.stringify(body) })

function toQuery(params) {
  const query = new URLSearchParams(Object.entries(params).filter(([, v]) => v !== '' && v != null)).toString()
  return query ? `?${query}` : ''
}

export const api = {
  register: (data) => post('/auth/register', data),
  login: (data) => post('/auth/login', data),
  getMovies: () => request('/movie/movies'),
  getMovie: (id) => request(`/movie/movies/${id}`),
  addMovie: (data) => post('/movie/movies', data),
  deleteMovie: (id) => request(`/movie/movies/${id}`, { method: 'DELETE' }),
  getSessions: (filters = {}) => request(`/MovieSession/sessions${toQuery(filters)}`),
  addSession: (data) => post('/MovieSession/sessions', data),
  updateSession: (id, data) => put(`/MovieSession/sessions/${id}`, data),
  deleteSession: (id) => request(`/MovieSession/sessions/${id}`, { method: 'DELETE' }),
}
