"use client";
import { useState, useEffect } from "react";
import { CheckCircle, BarChart3 } from "lucide-react";
import Checkins from "../components/Checkins";
import Analytics from "../components/Analytics";
import { useRouter } from "next/navigation";

export type Checkin = {
  id: string;
  mood: number;
  notes?: string;
  createdAt: string;
  username: string;
};

export default function Dashboard() {
  const [activeNav, setActiveNav] = useState<"checkins" | "analytics">(
    "checkins"
  );
  const [role, setRole] = useState<string | null>(null);
  const [authChecked, setAuthChecked] = useState(false);
  const [selectedCheckin, setSelectedCheckin] = useState<Checkin | null>(null);
  const router = useRouter();
  //  redirect if no token, otherwise load role
  //  We are not using jwt here for simplicity - memory token only
  //  it is basically stateless authentication
  useEffect(() => {
    const token = localStorage.getItem("authToken");
    if (!token) {
      router.replace("/login");
      return;
    }
    setRole(localStorage.getItem("role"));
    setAuthChecked(true);
  }, [router]);

  const handleEditFromAnalytics = (c: Checkin) => {
    setSelectedCheckin(c);
    setActiveNav("checkins");
  };

  const handleSaved = () => {
    // after save, clear selection so subsequent submit becomes POST again
    setSelectedCheckin(null);
  };

  const handleCancelEdit = () => {
    setSelectedCheckin(null);
  };

  // Avoid flicker before auth check completes
  if (!authChecked) return <div>Redirecting to Login</div>;

  return (
    <div className="min-h-screen bg-slate-50 flex flex-col">
      <header className="bg-white border-b border-slate-200">
        <div className="px-8 py-6">
          <div className="flex items-center justify-between mb-6">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-slate-900 rounded-lg flex items-center justify-center">
                <CheckCircle className="w-6 h-6 text-white" />
              </div>
              <div>
                <h1 className="text-lg font-bold text-slate-900">Dashboard</h1>
                <p className="text-xs text-slate-500">
                  Track your daily progress
                </p>
              </div>
            </div>
          </div>

          <nav>
            <ul className="flex items-center gap-2">
              <li>
                <button
                  onClick={() => setActiveNav("checkins")}
                  className={`flex items-center gap-2 px-4 py-2 rounded-lg font-medium transition-all ${
                    activeNav === "checkins"
                      ? "bg-slate-900 text-white"
                      : "text-slate-600 hover:bg-slate-100"
                  }`}
                >
                  <CheckCircle className="w-4 h-4" />
                  <span>Check-ins</span>
                </button>
              </li>

              {role === "manager" && (
                <li>
                  <button
                    onClick={() => setActiveNav("analytics")}
                    className={`flex items-center gap-2 px-4 py-2 rounded-lg font-medium transition-all ${
                      activeNav === "analytics"
                        ? "bg-slate-900 text-white"
                        : "text-slate-600 hover:bg-slate-100"
                    }`}
                  >
                    <BarChart3 className="w-4 h-4" />
                    <span>Analytics</span>
                  </button>
                </li>
              )}
            </ul>
          </nav>
        </div>
      </header>

      <main className="flex-1 overflow-auto p-8">
        {activeNav === "checkins" && (
          <Checkins
            selected={selectedCheckin}
            onSaved={handleSaved}
            onCancelEdit={handleCancelEdit}
          />
        )}
        {activeNav === "analytics" && role === "manager" && (
          <Analytics onEdit={handleEditFromAnalytics} />
        )}
      </main>
    </div>
  );
}
