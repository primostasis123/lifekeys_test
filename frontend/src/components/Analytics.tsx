"use client";
import { useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";

// Keep a local type so this file is self-contained
type Checkin = {
  id: string;
  mood: number;
  notes?: string;
  createdAt: string; // ISO string
  username: string;
};

type Props = {
  onEdit: (c: Checkin) => void;
};

export default function Analytics({ onEdit }: Props) {
  const [checkins, setCheckins] = useState<Checkin[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [filterUsername, setFilterUsername] = useState("");
  const [filterStart, setFilterStart] = useState(""); 
  const [filterEnd, setFilterEnd] = useState("");
  const router = useRouter();
  const apiUrl = `${process.env.NEXT_PUBLIC_API_URL}/checkins`;

  // Debounce username so we don't spam the API on every keystroke
  const [debouncedUser, setDebouncedUser] = useState(filterUsername);
  useEffect(() => {
    const t = setTimeout(() => setDebouncedUser(filterUsername.trim()), 300);
    return () => clearTimeout(t);
  }, [filterUsername]);

  // Simple validation: if both dates entered and start > end, block fetch and show error
  const dateRangeInvalid = useMemo(() => {
    if (!filterStart || !filterEnd) return false;
    return new Date(filterStart) > new Date(filterEnd);
  }, [filterStart, filterEnd]);

  // Fetch whenever filters change
  useEffect(() => {
    const fetchCheckins = async () => {
      if (dateRangeInvalid) {
        setError("Start date must be on or before End date.");
        setCheckins([]);
        return;
      }

      setLoading(true);
      setError("");

      try {
        const token = localStorage.getItem("authToken");
        if (!token) {
          router.push("/login");
          return;
        }

        // Build query: ?user=&from=&to=
        const url = new URL(apiUrl);
        if (debouncedUser) url.searchParams.set("user", debouncedUser);
        if (filterStart) url.searchParams.set("from", filterStart);
        if (filterEnd) url.searchParams.set("to", filterEnd);

        const res = await fetch(url.toString(), {
          headers: { Authorization: `Basic ${token}` },
        });

        if (!res.ok) throw new Error("Failed to fetch check-ins");
        const data = await res.json();
        setCheckins(data);
      } catch (err) {
        console.error(err);
        setError("Unable to load check-ins.");
        setCheckins([]);
      } finally {
        setLoading(false);
      }
    };

    fetchCheckins();
  }, [debouncedUser, filterStart, filterEnd, dateRangeInvalid, apiUrl, router]);

  const getMoodColor = (mood: number) => {
    if (mood === 1) return "bg-red-100 text-red-700";
    if (mood === 2) return "bg-orange-100 text-orange-700";
    if (mood === 3) return "bg-yellow-100 text-yellow-700";
    if (mood === 4) return "bg-blue-100 text-blue-700";
    if (mood === 5) return "bg-green-100 text-green-700";
    return "bg-slate-100 text-slate-700";
  };

  const getMoodLabel = (mood: number) =>
    ["", "Poor", "Fair", "Good", "Very Good", "Excellent"][mood];

  const clearFilters = () => {
    setFilterUsername("");
    setFilterStart("");
    setFilterEnd("");
  };

  return (
    <div>
      <h2 className="text-3xl font-bold text-slate-900 mb-6">All Check-ins</h2>

      <div className="bg-white rounded-xl shadow-sm p-6 border border-slate-200 mb-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label
              htmlFor="filterUsername"
              className="block text-sm font-medium text-slate-700 mb-2"
            >
              Filter by Username
            </label>
            <input
              id="filterUsername"
              type="text"
              value={filterUsername}
              onChange={(e) => setFilterUsername(e.target.value)}
              className="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-slate-900 focus:border-transparent outline-none"
              placeholder="Search username..."
            />
          </div>

          <div>
            <label
              htmlFor="filterStart"
              className="block text-sm font-medium text-slate-700 mb-2"
            >
              Start date
            </label>
            <input
              id="filterStart"
              type="date"
              value={filterStart}
              onChange={(e) => setFilterStart(e.target.value)}
              className="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-slate-900 focus:border-transparent outline-none"
            />
          </div>

          <div>
            <label
              htmlFor="filterEnd"
              className="block text-sm font-medium text-slate-700 mb-2"
            >
              End date
            </label>
            <input
              id="filterEnd"
              type="date"
              value={filterEnd}
              onChange={(e) => setFilterEnd(e.target.value)}
              className="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-slate-900 focus:border-transparent outline-none"
            />
          </div>
        </div>

        {(filterUsername || filterStart || filterEnd) && (
          <div className="mt-4 flex items-center gap-4">
            <button
              onClick={clearFilters}
              className="text-sm text-slate-600 hover:text-slate-900 font-medium"
            >
              Clear Filters
            </button>
            {dateRangeInvalid && (
              <span className="text-sm text-red-600">
                Start date must be on or before End date.
              </span>
            )}
          </div>
        )}
      </div>

      {loading ? (
        <p className="text-slate-600">Loading...</p>
      ) : error ? (
        <p className="text-red-600">{error}</p>
      ) : (
        <div className="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-slate-50 border-b border-slate-200">
                <tr>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-600 uppercase">
                    Username
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-600 uppercase">
                    Date
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-600 uppercase">
                    Mood
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-600 uppercase">
                    Notes
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-200">
                {checkins.map((c) => (
                  <tr key={c.id} className="hover:bg-slate-50 transition-colors">
                    <td className="px-6 py-4 text-sm font-medium text-slate-900">
                      <button
                        onClick={() => onEdit(c)}
                        className="text-blue-900 underline"
                        title="Edit this check-in"
                      >
                        {c.username || "N/A"}
                      </button>
                    </td>
                    <td className="px-6 py-4 text-sm text-slate-600">
                      {new Date(c.createdAt).toLocaleString()}
                    </td>
                    <td className="px-6 py-4">
                      <span
                        className={`inline-flex items-center gap-2 px-3 py-1 text-xs font-semibold rounded-full ${getMoodColor(
                          c.mood
                        )}`}
                      >
                        <span className="text-lg">{c.mood}</span>
                        <span>{getMoodLabel(c.mood)}</span>
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm text-slate-600 max-w-md truncate">
                      {c.notes || <span className="text-slate-400 italic">No notes</span>}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="px-6 py-3 text-sm text-slate-600 border-t border-slate-200">
            Showing {checkins.length} check-ins
          </div>
        </div>
      )}
    </div>
  );
}
