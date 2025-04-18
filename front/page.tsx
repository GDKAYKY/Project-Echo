import type React from "react"
import { Database, MonitorSmartphone, FileText, Globe, MessageSquare } from "lucide-react"
import { Input } from "@/components/ui/input"

export default function DatabaseSearch() {
  return (
    <div className="flex h-screen bg-black text-white">
      {/* Sidebar */}
      <div className="w-20 border-r border-gray-800">
        <div className="p-4 border-b border-gray-800">
          <div className="text-xl font-bold">©ECHO</div>
        </div>
        <div className="flex flex-col">
          <SidebarButton icon={<MessageSquare size={24} />} />
          <SidebarButton icon={<MonitorSmartphone size={24} />} />
          <SidebarButton icon={<Database size={24} />} />
          <SidebarButton icon={<FileText size={24} />} />
          <SidebarButton icon={<Globe size={24} />} />
        </div>
      </div>

      {/* Main Content */}
      <div className="flex-1 relative overflow-hidden">
        {/* World Map Background */}
        <div className="absolute inset-0 opacity-20">
          <WorldMapDots />
        </div>

        {/* Content */}
        <div className="relative z-10 flex flex-col items-center justify-center h-full">
          <div className="max-w-2xl w-full px-8">
            <h1 className="text-5xl font-bold mb-2">Database Search_</h1>
            <p className="text-gray-400 mb-8">Search Anyone, Anywhere, Anytime</p>
            <div className="w-48">
              <Input placeholder="Search_" className="bg-gray-200 text-black placeholder:text-gray-700" />
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

function SidebarButton({ icon }: { icon: React.ReactNode }) {
  return <button className="p-6 hover:bg-gray-900 transition-colors border-b border-gray-800">{icon}</button>
}

function WorldMapDots() {
  return (
    <div className="w-full h-full flex items-center justify-center">
      <div className="relative w-[1000px] h-[500px]">
        {/* This is a simplified representation of the dotted world map */}
        {/* In a real implementation, you might use an SVG or canvas for the dot pattern */}
        <div className="absolute inset-0 bg-[url('/world-map-dots.svg')] bg-no-repeat bg-contain bg-center" />
      </div>
    </div>
  )
}
