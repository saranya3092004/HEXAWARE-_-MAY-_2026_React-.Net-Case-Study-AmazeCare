import { useState, useEffect, useCallback } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { searchDoctors, getSpecializations } from '../api/doctors';
import { useAuth } from '../context/AuthContext';
import { isUserFacingError, GENERIC_ERROR } from '../utils/errors';
import './HomePage.css';

function PulseLine() {
  return (
    <svg className="hero-pulse" viewBox="0 0 400 60" fill="none" aria-hidden="true">
      <path
        d="M0 30 H140 L160 10 L180 50 L200 30 H400"
        stroke="currentColor"
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
}

function DoctorCard({ doctor, onBook }) {
  return (
    <div className="hp-doctor-card">
      <div className="hp-doctor-avatar">{doctor.name[0]}</div>
      <div className="hp-doctor-info">
        <h3 className="hp-doctor-name">{doctor.name}</h3>
        <span className="hp-doctor-spec">{doctor.specialization}</span>
        <div className="hp-doctor-meta">
          <span>{doctor.qualification}</span>
          <span className="hp-dot">·</span>
          <span>{doctor.designation}</span>
          <span className="hp-dot">·</span>
          <span>{doctor.experienceYears} yrs exp</span>
        </div>
      </div>
      <button className="hp-book-btn" onClick={() => onBook(doctor)}>
        Book appointment
      </button>
    </div>
  );
}

export default function HomePage() {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  const [search, setSearch] = useState({ name: '', specialty: '' });
  const [specializations, setSpecializations] = useState([]);
  const [doctors, setDoctors] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searched, setSearched] = useState(false);

  // Load specializations for the dropdown
  useEffect(() => {
    async function loadSpecializations() {
      try {
        const { data } = await getSpecializations();
        setSpecializations(data.data ?? []);
      } catch {
        setSpecializations([]);
      }
    }
    loadSpecializations();
  }, []);

  const load = useCallback(async (params = {}) => {
    setLoading(true);
    setError('');
    try {
      const { data } = await searchDoctors(params);
      setDoctors(data.data ?? []);
    } catch (err) {
      if (isUserFacingError(err)) setError(err.message);
      else { console.error(err); setError(GENERIC_ERROR); }
    } finally {
      setLoading(false);
    }
  }, []);

  // Load all doctors on mount
  useEffect(() => { load(); }, [load]);

  function handleSearch(e) {
    e.preventDefault();
    setSearched(true);
    load({
      name: search.name || undefined,
      specialization: search.specialty || undefined,
    });
  }

  function handleClear() {
    setSearch({ name: '', specialty: '' });
    setSearched(false);
    load();
  }

  function handleBook(doctor) {
    if (isAuthenticated) {
      navigate('/patient/book');
    } else {
      navigate('/login', {
        state: { bookingIntent: true, doctorName: doctor.name },
      });
    }
  }

  return (
    <div className="hp-shell">

      {/* ── Nav ─────────────────────────────────────── */}
      <header className="hp-nav">
        <span className="hp-nav-brand">AmazeCare</span>
        <div className="hp-nav-actions">
          {isAuthenticated ? (
            <button
              className="hp-nav-dashboard"
              onClick={() => navigate('/patient/appointments')}
            >
              My dashboard
            </button>
          ) : (
            <>
              <Link to="/login" className="hp-nav-login">Sign in</Link>
              <Link to="/register" className="hp-nav-register">Create account</Link>
            </>
          )}
        </div>
      </header>

      {/* ── Hero ────────────────────────────────────── */}
      <section className="hp-hero">
        <div className="hp-hero-inner">
          <PulseLine />
          <h1 className="hp-hero-title">
            Find a doctor,<br />book an appointment
          </h1>
          <p className="hp-hero-sub">
            Browse our team of specialists and book a consultation online.
            {!isAuthenticated && ' Sign in or create an account to book.'}
          </p>

          {/* Search bar — name + specialty dropdown */}
          <form className="hp-search-bar" onSubmit={handleSearch}>
            <div className="hp-search-field">
              <span className="hp-search-icon">🔍</span>
              <input
                type="text"
                className="hp-search-input"
                placeholder="Search by doctor name"
                value={search.name}
                onChange={(e) => setSearch({ ...search, name: e.target.value })}
              />
            </div>

            <div className="hp-search-divider" />

            <div className="hp-search-field">
              <span className="hp-search-icon">🩺</span>
              <select
                className="hp-search-select"
                value={search.specialty}
                onChange={(e) => setSearch({ ...search, specialty: e.target.value })}
              >
                <option value="">All specialties</option>
                {specializations.map((s) => (
                  <option key={s} value={s}>{s}</option>
                ))}
              </select>
            </div>

            <button type="submit" className="hp-search-btn">
              Find doctors
            </button>
          </form>

          {searched && (
            <button className="hp-clear-btn" type="button" onClick={handleClear}>
              Clear search — show all doctors
            </button>
          )}
        </div>
      </section>

      {/* ── Results header ───────────────────────────── */}
      <section className="hp-doctors">
        <div className="hp-doctors-inner">

          {loading ? (
            <div className="hp-spinner">Loading doctors…</div>
          ) : error ? (
            <div className="hp-error">{error}</div>
          ) : doctors.length === 0 ? (
            <div className="hp-empty">
              <p className="hp-empty-title">No doctors found</p>
              <p>
                Try a different name or specialty, or{' '}
                <button className="hp-text-btn" type="button" onClick={handleClear}>
                  view all doctors
                </button>.
              </p>
            </div>
          ) : (
            <>
              <div className="hp-results-header">
                <p className="hp-count">
                  {searched
                    ? `${doctors.length} result${doctors.length !== 1 ? 's' : ''} found`
                    : `${doctors.length} doctor${doctors.length !== 1 ? 's' : ''} available`}
                </p>
                {searched && (
                  <button className="hp-text-btn" type="button" onClick={handleClear}>
                    Clear filters
                  </button>
                )}
              </div>

              <div className="hp-doctor-list-wrap">
                <div className="hp-doctor-list">
                  {doctors.map((d) => (
                    <DoctorCard key={d.doctorId} doctor={d} onBook={handleBook} />
                  ))}
                </div>
              </div>
            </>
          )}

        </div>
      </section>

      {/* ── Footer ──────────────────────────────────── */}
      <footer className="hp-footer">
        <div className="hp-footer-inner">

          <div className="hp-footer-brand">
            <span className="hp-footer-logo">AmazeCare</span>
            <p className="hp-footer-tagline">
              Appointments, records, and care — in one place.
            </p>
          </div>

          <div className="hp-footer-links">
            <div className="hp-footer-col">
              <span className="hp-footer-col-title">Patients</span>
              <Link to="/register">Create account</Link>
              <Link to="/login">Sign in</Link>
              <Link to="/patient/appointments">My appointments</Link>
            </div>
            <div className="hp-footer-col">
              <span className="hp-footer-col-title">Doctors</span>
              <Link to="/login">Doctor login</Link>
              <Link to="/doctor/appointments">My schedule</Link>
            </div>
            <div className="hp-footer-col">
              <span className="hp-footer-col-title">About</span>
              <span className="hp-footer-plain">
                AmazeCare is a hospital appointment management system built to streamline patient care.
              </span>
            </div>
          </div>

        </div>

        <div className="hp-footer-bottom">
          <span>© 2026 AmazeCare. All rights reserved.</span>
          <span>Powered by Hexaware Technologies</span>
        </div>
      </footer>

    </div>
  );
}